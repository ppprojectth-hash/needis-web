using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("About.View")]
public class AboutHistoryController : Controller
{
    private readonly AppDbContext _db;
    private string CurrentUser => User.Identity?.Name ?? "system";

    public AboutHistoryController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword)
    {
        ViewData["Title"] = "Company History";
        ViewBag.Keyword   = keyword;

        var query = _db.AboutCompanyHistories.AsNoTracking()
            .Where(h => !h.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(h =>
                (h.TitleTH != null && h.TitleTH.Contains(keyword)) ||
                (h.TitleEN != null && h.TitleEN.Contains(keyword)));

        var list = await query
            .OrderBy(h => h.DisplayOrder)
            .ThenBy(h => h.Year)
            .ToListAsync();

        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add History Event";
        return View(new AboutHistoryViewModel
        {
            Year     = DateTime.UtcNow.Year,
            Position = "left",
            IsActive = true,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AboutHistoryViewModel vm)
    {
        ViewData["Title"] = "Add History Event";

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.Position != "left" && vm.Position != "right")
        {
            ModelState.AddModelError(nameof(vm.Position), "Position must be 'left' or 'right'.");
            return View(vm);
        }

        _db.AboutCompanyHistories.Add(new AboutCompanyHistory
        {
            Year          = vm.Year,
            TitleTH       = vm.TitleTH,
            TitleEN       = vm.TitleEN,
            DescriptionTH = vm.DescriptionTH,
            DescriptionEN = vm.DescriptionEN,
            Position      = vm.Position,
            DisplayOrder  = vm.DisplayOrder,
            IsActive      = vm.IsActive,
            CreatedAt     = DateTime.UtcNow,
            CreatedBy     = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "History event added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit History Event";
        var h = await _db.AboutCompanyHistories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (h is null) return NotFound();

        return View(new AboutHistoryViewModel
        {
            Id            = h.Id,
            Year          = h.Year,
            TitleTH       = h.TitleTH,
            TitleEN       = h.TitleEN,
            DescriptionTH = h.DescriptionTH,
            DescriptionEN = h.DescriptionEN,
            Position      = h.Position,
            DisplayOrder  = h.DisplayOrder,
            IsActive      = h.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AboutHistoryViewModel vm)
    {
        ViewData["Title"] = "Edit History Event";

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.Position != "left" && vm.Position != "right")
        {
            ModelState.AddModelError(nameof(vm.Position), "Position must be 'left' or 'right'.");
            return View(vm);
        }

        var entity = await _db.AboutCompanyHistories.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        entity.Year          = vm.Year;
        entity.TitleTH       = vm.TitleTH;
        entity.TitleEN       = vm.TitleEN;
        entity.DescriptionTH = vm.DescriptionTH;
        entity.DescriptionEN = vm.DescriptionEN;
        entity.Position      = vm.Position;
        entity.DisplayOrder  = vm.DisplayOrder;
        entity.IsActive      = vm.IsActive;
        entity.UpdatedAt     = DateTime.UtcNow;
        entity.UpdatedBy     = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "History event updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.AboutCompanyHistories.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "History event deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
