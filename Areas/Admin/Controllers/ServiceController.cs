using System.Text.RegularExpressions;
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
public class ServiceController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ServiceController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    private static string NormalizeSlug(string input)
    {
        var slug = input.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = Regex.Replace(slug, @"-{2,}", "-");
        return slug.Trim('-');
    }

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword)
    {
        ViewData["Title"] = "Services";
        ViewBag.Keyword   = keyword;

        var query = _db.Services.AsNoTracking()
            .Where(s => !s.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(s =>
                s.ServiceCode.Contains(keyword) ||
                s.ServiceSlug.Contains(keyword) ||
                (s.ServiceNameTH != null && s.ServiceNameTH.Contains(keyword)) ||
                (s.ServiceNameEN != null && s.ServiceNameEN.Contains(keyword)));

        var list = await query
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.ServiceNameEN)
            .Select(s => new
            {
                s.Id,
                s.ServiceCode,
                s.ServiceSlug,
                s.ServiceNameEN,
                s.ServiceNameTH,
                s.CoverImageUrl,
                s.IsFeatured,
                s.IsActive,
                s.DisplayOrder,
                SectionCount = s.DetailSections.Count(x => !x.IsDelete),
                CardCount    = s.ContactCards.Count(x => !x.IsDelete),
            })
            .ToListAsync();

        return View(list);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Create Service";
        return View(new ServiceViewModel { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceViewModel vm)
    {
        ViewData["Title"] = "Create Service";

        vm.ServiceSlug = NormalizeSlug(vm.ServiceSlug);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.Services.AnyAsync(s => s.ServiceCode == vm.ServiceCode && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.ServiceCode), $"ServiceCode '{vm.ServiceCode}' already exists.");
            return View(vm);
        }

        if (await _db.Services.AnyAsync(s => s.ServiceSlug == vm.ServiceSlug && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.ServiceSlug), $"Slug '{vm.ServiceSlug}' already exists.");
            return View(vm);
        }

        string? coverPath  = null;
        string? bannerPath = null;
        string? iconPath   = null;

        if (vm.CoverImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.CoverImageFile, _env.WebRootPath, "uploads/services/covers");
            if (!ok) { ModelState.AddModelError(nameof(vm.CoverImageFile), err); return View(vm); }
            coverPath = path;
        }

        if (vm.BannerImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BannerImageFile, _env.WebRootPath, "uploads/services/banners");
            if (!ok) { ModelState.AddModelError(nameof(vm.BannerImageFile), err); return View(vm); }
            bannerPath = path;
        }

        if (vm.IconFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.IconFile, _env.WebRootPath, "uploads/services/icons");
            if (!ok) { ModelState.AddModelError(nameof(vm.IconFile), err); return View(vm); }
            iconPath = path;
        }

        _db.Services.Add(new Models.Service
        {
            ServiceCode        = vm.ServiceCode,
            ServiceSlug        = vm.ServiceSlug,
            ServiceNameTH      = vm.ServiceNameTH,
            ServiceNameEN      = vm.ServiceNameEN,
            ShortDescriptionTH = vm.ShortDescriptionTH,
            ShortDescriptionEN = vm.ShortDescriptionEN,
            FullDescriptionTH  = vm.FullDescriptionTH,
            FullDescriptionEN  = vm.FullDescriptionEN,
            CoverImageUrl      = coverPath,
            BannerImageUrl     = bannerPath,
            IconUrl            = iconPath,
            DetailTitleTH      = vm.DetailTitleTH,
            DetailTitleEN      = vm.DetailTitleEN,
            DetailSubtitleTH   = vm.DetailSubtitleTH,
            DetailSubtitleEN   = vm.DetailSubtitleEN,
            DisplayOrder       = vm.DisplayOrder,
            IsFeatured         = vm.IsFeatured,
            IsActive           = vm.IsActive,
            CreatedAt          = DateTime.UtcNow,
            CreatedBy          = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Service created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Service";
        var svc = await _db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (svc is null) return NotFound();

        return View(new ServiceViewModel
        {
            Id                    = svc.Id,
            ServiceCode           = svc.ServiceCode,
            ServiceSlug           = svc.ServiceSlug,
            ServiceNameTH         = svc.ServiceNameTH,
            ServiceNameEN         = svc.ServiceNameEN,
            ShortDescriptionTH    = svc.ShortDescriptionTH,
            ShortDescriptionEN    = svc.ShortDescriptionEN,
            FullDescriptionTH     = svc.FullDescriptionTH,
            FullDescriptionEN     = svc.FullDescriptionEN,
            ExistingCoverImageUrl  = svc.CoverImageUrl,
            ExistingBannerImageUrl = svc.BannerImageUrl,
            ExistingIconUrl        = svc.IconUrl,
            DetailTitleTH         = svc.DetailTitleTH,
            DetailTitleEN         = svc.DetailTitleEN,
            DetailSubtitleTH      = svc.DetailSubtitleTH,
            DetailSubtitleEN      = svc.DetailSubtitleEN,
            DisplayOrder          = svc.DisplayOrder,
            IsFeatured            = svc.IsFeatured,
            IsActive              = svc.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceViewModel vm)
    {
        ViewData["Title"] = "Edit Service";

        vm.ServiceSlug = NormalizeSlug(vm.ServiceSlug);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.Services.AnyAsync(s => s.ServiceCode == vm.ServiceCode && s.Id != id && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.ServiceCode), $"ServiceCode '{vm.ServiceCode}' already exists.");
            return View(vm);
        }

        if (await _db.Services.AnyAsync(s => s.ServiceSlug == vm.ServiceSlug && s.Id != id && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.ServiceSlug), $"Slug '{vm.ServiceSlug}' already exists.");
            return View(vm);
        }

        var entity = await _db.Services.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.CoverImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.CoverImageFile, _env.WebRootPath, "uploads/services/covers");
            if (!ok) { ModelState.AddModelError(nameof(vm.CoverImageFile), err); return View(vm); }
            entity.CoverImageUrl = path;
        }

        if (vm.BannerImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BannerImageFile, _env.WebRootPath, "uploads/services/banners");
            if (!ok) { ModelState.AddModelError(nameof(vm.BannerImageFile), err); return View(vm); }
            entity.BannerImageUrl = path;
        }

        if (vm.IconFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.IconFile, _env.WebRootPath, "uploads/services/icons");
            if (!ok) { ModelState.AddModelError(nameof(vm.IconFile), err); return View(vm); }
            entity.IconUrl = path;
        }

        entity.ServiceCode        = vm.ServiceCode;
        entity.ServiceSlug        = vm.ServiceSlug;
        entity.ServiceNameTH      = vm.ServiceNameTH;
        entity.ServiceNameEN      = vm.ServiceNameEN;
        entity.ShortDescriptionTH = vm.ShortDescriptionTH;
        entity.ShortDescriptionEN = vm.ShortDescriptionEN;
        entity.FullDescriptionTH  = vm.FullDescriptionTH;
        entity.FullDescriptionEN  = vm.FullDescriptionEN;
        entity.DetailTitleTH      = vm.DetailTitleTH;
        entity.DetailTitleEN      = vm.DetailTitleEN;
        entity.DetailSubtitleTH   = vm.DetailSubtitleTH;
        entity.DetailSubtitleEN   = vm.DetailSubtitleEN;
        entity.DisplayOrder       = vm.DisplayOrder;
        entity.IsFeatured         = vm.IsFeatured;
        entity.IsActive           = vm.IsActive;
        entity.UpdatedAt          = DateTime.UtcNow;
        entity.UpdatedBy          = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Service updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Details ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var svc = await _db.Services.AsNoTracking()
            .Where(x => x.Id == id && !x.IsDelete)
            .Select(x => new
            {
                x.Id,
                x.ServiceCode,
                x.ServiceSlug,
                x.ServiceNameEN,
                x.ServiceNameTH,
                x.ShortDescriptionEN,
                x.ShortDescriptionTH,
                x.CoverImageUrl,
                x.BannerImageUrl,
                x.IconUrl,
                x.DetailTitleEN,
                x.DetailTitleTH,
                x.DetailSubtitleEN,
                x.DetailSubtitleTH,
                x.IsFeatured,
                x.IsActive,
                x.DisplayOrder,
                x.CreatedAt,
                x.CreatedBy,
                x.UpdatedAt,
                x.UpdatedBy,
                SectionCount = x.DetailSections.Count(s => !s.IsDelete),
                CardCount    = x.ContactCards.Count(c => !c.IsDelete),
            })
            .FirstOrDefaultAsync();

        if (svc is null) return NotFound();

        ViewData["Title"] = $"Service Details — {svc.ServiceNameEN}";
        return View(svc);
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Services.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Service deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
