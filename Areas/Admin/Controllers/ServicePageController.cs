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
[RequirePermission("Service.View")]
public class ServicePageController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ServicePageController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Service Page Settings";

        var list = await _db.ServicePages.AsNoTracking()
            .Where(p => !p.IsDelete)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.PageKey)
            .ToListAsync();

        return View(list);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Create Service Page Setting";
        return View(new ServicePageViewModel { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServicePageViewModel vm)
    {
        ViewData["Title"] = "Create Service Page Setting";

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.ServicePages.AnyAsync(p => p.PageKey == vm.PageKey && !p.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.PageKey), $"PageKey '{vm.PageKey}' already exists.");
            return View(vm);
        }

        string? bgPath = null;
        if (vm.BackgroundImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BackgroundImageFile, _env.WebRootPath, "uploads/services");
            if (!ok) { ModelState.AddModelError(nameof(vm.BackgroundImageFile), err); return View(vm); }
            bgPath = path;
        }

        _db.ServicePages.Add(new ServicePage
        {
            PageKey             = vm.PageKey,
            TitleTH             = vm.TitleTH,
            TitleEN             = vm.TitleEN,
            SubtitleTH          = vm.SubtitleTH,
            SubtitleEN          = vm.SubtitleEN,
            DescriptionTH       = vm.DescriptionTH,
            DescriptionEN       = vm.DescriptionEN,
            BackgroundImageUrl  = bgPath,
            DisplayOrder        = vm.DisplayOrder,
            IsActive            = vm.IsActive,
            CreatedAt           = DateTime.UtcNow,
            CreatedBy           = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Service page setting created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Service Page Setting";
        var p = await _db.ServicePages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (p is null) return NotFound();

        return View(new ServicePageViewModel
        {
            Id                        = p.Id,
            PageKey                   = p.PageKey,
            TitleTH                   = p.TitleTH,
            TitleEN                   = p.TitleEN,
            SubtitleTH                = p.SubtitleTH,
            SubtitleEN                = p.SubtitleEN,
            DescriptionTH             = p.DescriptionTH,
            DescriptionEN             = p.DescriptionEN,
            ExistingBackgroundImageUrl = p.BackgroundImageUrl,
            DisplayOrder              = p.DisplayOrder,
            IsActive                  = p.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServicePageViewModel vm)
    {
        ViewData["Title"] = "Edit Service Page Setting";

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.ServicePages.AnyAsync(p => p.PageKey == vm.PageKey && p.Id != id && !p.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.PageKey), $"PageKey '{vm.PageKey}' already exists.");
            return View(vm);
        }

        var entity = await _db.ServicePages.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.BackgroundImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BackgroundImageFile, _env.WebRootPath, "uploads/services");
            if (!ok) { ModelState.AddModelError(nameof(vm.BackgroundImageFile), err); return View(vm); }
            entity.BackgroundImageUrl = path;
        }

        entity.PageKey        = vm.PageKey;
        entity.TitleTH        = vm.TitleTH;
        entity.TitleEN        = vm.TitleEN;
        entity.SubtitleTH     = vm.SubtitleTH;
        entity.SubtitleEN     = vm.SubtitleEN;
        entity.DescriptionTH  = vm.DescriptionTH;
        entity.DescriptionEN  = vm.DescriptionEN;
        entity.DisplayOrder   = vm.DisplayOrder;
        entity.IsActive       = vm.IsActive;
        entity.UpdatedAt      = DateTime.UtcNow;
        entity.UpdatedBy      = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Service page setting updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ServicePages.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Service page setting deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
