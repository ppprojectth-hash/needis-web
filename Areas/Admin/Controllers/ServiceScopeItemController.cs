using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Service.Edit")]
public class ServiceScopeItemController : Controller
{
    private readonly AppDbContext _db;

    public ServiceScopeItemController(AppDbContext db)
    {
        _db = db;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(int sectionId)
    {
        var detailSec = await _db.ServiceDetailSections.AsNoTracking()
            .Include(s => s.Service)
            .FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDelete);
        if (detailSec is null) return NotFound();

        ViewData["Title"]   = $"Scope Items — {detailSec.SectionTitleEN ?? detailSec.SectionKey}";
        ViewBag.DetailSection = detailSec;

        var items = await _db.ServiceScopeItems.AsNoTracking()
            .Where(i => i.ServiceDetailSectionId == sectionId && !i.IsDelete)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync();

        return View(items);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Create(int sectionId)
    {
        var detailSec = await _db.ServiceDetailSections.AsNoTracking()
            .Include(s => s.Service)
            .FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDelete);
        if (detailSec is null) return NotFound();

        ViewData["Title"]   = "Add Scope Item";
        ViewBag.DetailSection = detailSec;
        return View(new ServiceScopeItemViewModel { ServiceDetailSectionId = sectionId, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceScopeItemViewModel vm)
    {
        var detailSec = await _db.ServiceDetailSections.AsNoTracking()
            .Include(s => s.Service)
            .FirstOrDefaultAsync(s => s.Id == vm.ServiceDetailSectionId && !s.IsDelete);
        if (detailSec is null) return NotFound();

        ViewData["Title"]   = "Add Scope Item";
        ViewBag.DetailSection = detailSec;

        if (!ModelState.IsValid)
            return View(vm);

        _db.ServiceScopeItems.Add(new ServiceScopeItem
        {
            ServiceDetailSectionId = vm.ServiceDetailSectionId,
            ItemTitleTH            = vm.ItemTitleTH,
            ItemTitleEN            = vm.ItemTitleEN,
            ItemDescriptionTH      = vm.ItemDescriptionTH,
            ItemDescriptionEN      = vm.ItemDescriptionEN,
            IconUrl                = vm.IconUrl,
            DisplayOrder           = vm.DisplayOrder,
            IsActive               = vm.IsActive,
            CreatedAt              = DateTime.UtcNow,
            CreatedBy              = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Scope item added.";
        return RedirectToAction(nameof(Index), new { sectionId = vm.ServiceDetailSectionId });
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Scope Item";
        var item = await _db.ServiceScopeItems.AsNoTracking()
            .Include(i => i.ServiceDetailSection)
                .ThenInclude(s => s!.Service)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDelete);
        if (item is null) return NotFound();

        ViewBag.DetailSection = item.ServiceDetailSection;
        return View(new ServiceScopeItemViewModel
        {
            Id                     = item.Id,
            ServiceDetailSectionId = item.ServiceDetailSectionId,
            ItemTitleTH            = item.ItemTitleTH,
            ItemTitleEN            = item.ItemTitleEN,
            ItemDescriptionTH      = item.ItemDescriptionTH,
            ItemDescriptionEN      = item.ItemDescriptionEN,
            IconUrl                = item.IconUrl,
            DisplayOrder           = item.DisplayOrder,
            IsActive               = item.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceScopeItemViewModel vm)
    {
        ViewData["Title"] = "Edit Scope Item";

        if (!ModelState.IsValid)
        {
            var detailSecForView = await _db.ServiceDetailSections.AsNoTracking()
                .Include(s => s.Service)
                .FirstOrDefaultAsync(s => s.Id == vm.ServiceDetailSectionId);
            ViewBag.DetailSection = detailSecForView;
            return View(vm);
        }

        var entity = await _db.ServiceScopeItems.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        entity.ItemTitleTH       = vm.ItemTitleTH;
        entity.ItemTitleEN       = vm.ItemTitleEN;
        entity.ItemDescriptionTH = vm.ItemDescriptionTH;
        entity.ItemDescriptionEN = vm.ItemDescriptionEN;
        entity.IconUrl           = vm.IconUrl;
        entity.DisplayOrder      = vm.DisplayOrder;
        entity.IsActive          = vm.IsActive;
        entity.UpdatedAt         = DateTime.UtcNow;
        entity.UpdatedBy         = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Scope item updated.";
        return RedirectToAction(nameof(Index), new { sectionId = entity.ServiceDetailSectionId });
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ServiceScopeItems.FindAsync(id);
        if (entity is not null)
        {
            var sectionId    = entity.ServiceDetailSectionId;
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Scope item deleted.";
            return RedirectToAction(nameof(Index), new { sectionId });
        }
        return RedirectToAction(nameof(Index), new { sectionId = 0 });
    }
}
