using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Service.Edit")]
public class ServiceDetailSectionController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ServiceDetailSectionController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(int serviceId)
    {
        var svc = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == serviceId && !x.IsDelete);
        if (svc is null) return NotFound();

        ViewData["Title"] = $"Sections — {svc.ServiceNameEN ?? svc.ServiceCode}";
        ViewBag.Service   = svc;

        var items = await _db.ServiceDetailSections.AsNoTracking()
            .Where(s => s.ServiceId == serviceId && !s.IsDelete)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.SectionKey)
            .ToListAsync();

        return View(items);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Create(int serviceId)
    {
        var svc = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == serviceId && !x.IsDelete);
        if (svc is null) return NotFound();

        ViewData["Title"] = "Add Section";
        ViewBag.Service   = svc;
        return View(new ServiceDetailSectionViewModel { ServiceId = serviceId, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceDetailSectionViewModel vm)
    {
        var svc = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.ServiceId && !x.IsDelete);
        if (svc is null) return NotFound();

        ViewData["Title"] = "Add Section";
        ViewBag.Service   = svc;

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.ServiceDetailSections.AnyAsync(s => s.ServiceId == vm.ServiceId && s.SectionKey == vm.SectionKey && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.SectionKey), $"SectionKey '{vm.SectionKey}' already exists for this service.");
            return View(vm);
        }

        string? imgPath = null;
        if (vm.SectionImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.SectionImageFile, _env.WebRootPath, "uploads/services/sections");
            if (!ok) { ModelState.AddModelError(nameof(vm.SectionImageFile), err); return View(vm); }
            imgPath = path;
        }

        _db.ServiceDetailSections.Add(new ServiceDetailSection
        {
            ServiceId            = vm.ServiceId,
            SectionKey           = vm.SectionKey,
            SectionTitleTH       = vm.SectionTitleTH,
            SectionTitleEN       = vm.SectionTitleEN,
            SectionSubtitleTH    = vm.SectionSubtitleTH,
            SectionSubtitleEN    = vm.SectionSubtitleEN,
            SectionDescriptionTH = vm.SectionDescriptionTH,
            SectionDescriptionEN = vm.SectionDescriptionEN,
            SectionImageUrl      = imgPath,
            LayoutType           = vm.LayoutType,
            DisplayOrder         = vm.DisplayOrder,
            IsActive             = vm.IsActive,
            CreatedAt            = DateTime.UtcNow,
            CreatedBy            = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Section created successfully.";
        return RedirectToAction(nameof(Index), new { serviceId = vm.ServiceId });
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Section";
        var sec = await _db.ServiceDetailSections.AsNoTracking()
            .Include(x => x.Service)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (sec is null) return NotFound();

        ViewBag.Service = sec.Service;
        return View(new ServiceDetailSectionViewModel
        {
            Id                   = sec.Id,
            ServiceId            = sec.ServiceId,
            SectionKey           = sec.SectionKey,
            SectionTitleTH       = sec.SectionTitleTH,
            SectionTitleEN       = sec.SectionTitleEN,
            SectionSubtitleTH    = sec.SectionSubtitleTH,
            SectionSubtitleEN    = sec.SectionSubtitleEN,
            SectionDescriptionTH = sec.SectionDescriptionTH,
            SectionDescriptionEN = sec.SectionDescriptionEN,
            ExistingImageUrl     = sec.SectionImageUrl,
            LayoutType           = sec.LayoutType,
            DisplayOrder         = sec.DisplayOrder,
            IsActive             = sec.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceDetailSectionViewModel vm)
    {
        ViewData["Title"] = "Edit Section";

        if (!ModelState.IsValid)
        {
            var svcForView = await _db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.ServiceId);
            ViewBag.Service = svcForView;
            return View(vm);
        }

        if (await _db.ServiceDetailSections.AnyAsync(s => s.ServiceId == vm.ServiceId && s.SectionKey == vm.SectionKey && s.Id != id && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.SectionKey), $"SectionKey '{vm.SectionKey}' already exists for this service.");
            var svcForView = await _db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.ServiceId);
            ViewBag.Service = svcForView;
            return View(vm);
        }

        var entity = await _db.ServiceDetailSections.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.SectionImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.SectionImageFile, _env.WebRootPath, "uploads/services/sections");
            if (!ok)
            {
                ModelState.AddModelError(nameof(vm.SectionImageFile), err);
                var svcForView = await _db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.ServiceId);
                ViewBag.Service = svcForView;
                return View(vm);
            }
            entity.SectionImageUrl = path;
        }

        entity.SectionKey           = vm.SectionKey;
        entity.SectionTitleTH       = vm.SectionTitleTH;
        entity.SectionTitleEN       = vm.SectionTitleEN;
        entity.SectionSubtitleTH    = vm.SectionSubtitleTH;
        entity.SectionSubtitleEN    = vm.SectionSubtitleEN;
        entity.SectionDescriptionTH = vm.SectionDescriptionTH;
        entity.SectionDescriptionEN = vm.SectionDescriptionEN;
        entity.LayoutType           = vm.LayoutType;
        entity.DisplayOrder         = vm.DisplayOrder;
        entity.IsActive             = vm.IsActive;
        entity.UpdatedAt            = DateTime.UtcNow;
        entity.UpdatedBy            = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Section updated successfully.";
        return RedirectToAction(nameof(Index), new { serviceId = entity.ServiceId });
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ServiceDetailSections.FindAsync(id);
        if (entity is not null)
        {
            var serviceId    = entity.ServiceId;
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Section deleted.";
            return RedirectToAction(nameof(Index), new { serviceId });
        }
        return RedirectToAction(nameof(Index), new { serviceId = 0 });
    }
}
