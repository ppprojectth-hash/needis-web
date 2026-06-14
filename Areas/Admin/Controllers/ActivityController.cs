using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.ViewModels.Admin;
using ActivityModel = Needis.Web.Models.Activity;
using Needis.Web.Models;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Activity.View")]
public class ActivityController : Controller
{
    private readonly AppDbContext        _db;
    private readonly IWebHostEnvironment _env;

    public ActivityController(AppDbContext db, IWebHostEnvironment env)
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
    public async Task<IActionResult> Index(string? keyword, string? tag)
    {
        ViewData["Title"] = "Activities";
        ViewBag.Keyword   = keyword;
        ViewBag.Tag       = tag;
        ViewBag.AllTags   = await _db.ActivityTags.AsNoTracking()
            .Where(t => !t.IsDelete && t.IsActive)
            .OrderBy(t => t.DisplayOrder)
            .ToListAsync();

        var query = _db.Activities.AsNoTracking()
            .Where(a => !a.IsDelete)
            .Include(a => a.ActivityTagMaps).ThenInclude(m => m.ActivityTag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(a =>
                a.ActivitySlug.Contains(keyword) ||
                (a.ActivityTitleEN != null && a.ActivityTitleEN.Contains(keyword)) ||
                (a.ActivityTitleTH != null && a.ActivityTitleTH.Contains(keyword)) ||
                (a.AuthorName      != null && a.AuthorName.Contains(keyword))      ||
                (a.LocationEN      != null && a.LocationEN.Contains(keyword))      ||
                (a.LocationTH      != null && a.LocationTH.Contains(keyword)));

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(a => a.ActivityTagMaps.Any(m =>
                !m.ActivityTag!.IsDelete && m.ActivityTag.TagKey == tag));

        var list = await query
            .OrderByDescending(a => a.PublishedDate.HasValue ? a.PublishedDate : null)
            .ThenByDescending(a => a.ActivityDate.HasValue ? a.ActivityDate : null)
            .ThenBy(a => a.DisplayOrder)
            .ToListAsync();

        return View(list);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Create Activity";
        return View(new ActivityViewModel { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityViewModel vm)
    {
        ViewData["Title"] = "Create Activity";

        vm.ActivitySlug = NormalizeSlug(vm.ActivitySlug);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.Activities.AnyAsync(a => a.ActivitySlug == vm.ActivitySlug && !a.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.ActivitySlug), $"Slug '{vm.ActivitySlug}' already exists.");
            return View(vm);
        }

        string? coverPath  = null;
        string? bannerPath = null;

        if (vm.CoverImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.CoverImageFile, _env.WebRootPath, "uploads/activity/covers");
            if (!ok) { ModelState.AddModelError(nameof(vm.CoverImageFile), err); return View(vm); }
            coverPath = path;
        }

        if (vm.BannerImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BannerImageFile, _env.WebRootPath, "uploads/activity/banners");
            if (!ok) { ModelState.AddModelError(nameof(vm.BannerImageFile), err); return View(vm); }
            bannerPath = path;
        }

        var activity = new ActivityModel
        {
            ActivitySlug       = vm.ActivitySlug,
            ActivityTitleTH    = vm.ActivityTitleTH,
            ActivityTitleEN    = vm.ActivityTitleEN,
            ShortDescriptionTH = vm.ShortDescriptionTH,
            ShortDescriptionEN = vm.ShortDescriptionEN,
            SummaryTH          = vm.SummaryTH,
            SummaryEN          = vm.SummaryEN,
            ContentPreviewTH   = vm.ContentPreviewTH,
            ContentPreviewEN   = vm.ContentPreviewEN,
            CoverImageUrl      = coverPath,
            BannerImageUrl     = bannerPath,
            ActivityDate       = vm.ActivityDate.HasValue
                ? DateTime.SpecifyKind(vm.ActivityDate.Value, DateTimeKind.Utc) : null,
            PublishedDate      = vm.PublishedDate.HasValue
                ? DateTime.SpecifyKind(vm.PublishedDate.Value, DateTimeKind.Utc) : null,
            LocationTH         = vm.LocationTH,
            LocationEN         = vm.LocationEN,
            AuthorName         = vm.AuthorName,
            IsFeatured         = vm.IsFeatured,
            IsPublished        = vm.IsPublished,
            DisplayOrder       = vm.DisplayOrder,
            MetaTitleTH        = vm.MetaTitleTH,
            MetaTitleEN        = vm.MetaTitleEN,
            MetaDescriptionTH  = vm.MetaDescriptionTH,
            MetaDescriptionEN  = vm.MetaDescriptionEN,
            IsActive           = vm.IsActive,
            CreatedAt          = DateTime.UtcNow,
            CreatedBy          = CurrentUser,
        };

        _db.Activities.Add(activity);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Activity created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Activity";
        var a = await _db.Activities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (a is null) return NotFound();

        return View(new ActivityViewModel
        {
            Id                    = a.Id,
            ActivitySlug          = a.ActivitySlug,
            ActivityTitleTH       = a.ActivityTitleTH,
            ActivityTitleEN       = a.ActivityTitleEN,
            ShortDescriptionTH    = a.ShortDescriptionTH,
            ShortDescriptionEN    = a.ShortDescriptionEN,
            SummaryTH             = a.SummaryTH,
            SummaryEN             = a.SummaryEN,
            ContentPreviewTH      = a.ContentPreviewTH,
            ContentPreviewEN      = a.ContentPreviewEN,
            ExistingCoverImageUrl  = a.CoverImageUrl,
            ExistingBannerImageUrl = a.BannerImageUrl,
            ActivityDate          = a.ActivityDate,
            PublishedDate         = a.PublishedDate,
            LocationTH            = a.LocationTH,
            LocationEN            = a.LocationEN,
            AuthorName            = a.AuthorName,
            IsFeatured            = a.IsFeatured,
            IsPublished           = a.IsPublished,
            DisplayOrder          = a.DisplayOrder,
            MetaTitleTH           = a.MetaTitleTH,
            MetaTitleEN           = a.MetaTitleEN,
            MetaDescriptionTH     = a.MetaDescriptionTH,
            MetaDescriptionEN     = a.MetaDescriptionEN,
            IsActive              = a.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActivityViewModel vm)
    {
        ViewData["Title"] = "Edit Activity";

        vm.ActivitySlug = NormalizeSlug(vm.ActivitySlug);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.Activities.AnyAsync(a => a.ActivitySlug == vm.ActivitySlug && a.Id != id && !a.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.ActivitySlug), $"Slug '{vm.ActivitySlug}' already exists.");
            return View(vm);
        }

        var entity = await _db.Activities.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.CoverImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.CoverImageFile, _env.WebRootPath, "uploads/activity/covers");
            if (!ok) { ModelState.AddModelError(nameof(vm.CoverImageFile), err); return View(vm); }
            entity.CoverImageUrl = path;
        }

        if (vm.BannerImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BannerImageFile, _env.WebRootPath, "uploads/activity/banners");
            if (!ok) { ModelState.AddModelError(nameof(vm.BannerImageFile), err); return View(vm); }
            entity.BannerImageUrl = path;
        }

        entity.ActivitySlug       = vm.ActivitySlug;
        entity.ActivityTitleTH    = vm.ActivityTitleTH;
        entity.ActivityTitleEN    = vm.ActivityTitleEN;
        entity.ShortDescriptionTH = vm.ShortDescriptionTH;
        entity.ShortDescriptionEN = vm.ShortDescriptionEN;
        entity.SummaryTH          = vm.SummaryTH;
        entity.SummaryEN          = vm.SummaryEN;
        entity.ContentPreviewTH   = vm.ContentPreviewTH;
        entity.ContentPreviewEN   = vm.ContentPreviewEN;
        entity.ActivityDate       = vm.ActivityDate.HasValue
            ? DateTime.SpecifyKind(vm.ActivityDate.Value, DateTimeKind.Utc) : null;
        entity.PublishedDate      = vm.PublishedDate.HasValue
            ? DateTime.SpecifyKind(vm.PublishedDate.Value, DateTimeKind.Utc) : null;
        entity.LocationTH         = vm.LocationTH;
        entity.LocationEN         = vm.LocationEN;
        entity.AuthorName         = vm.AuthorName;
        entity.IsFeatured         = vm.IsFeatured;
        entity.IsPublished        = vm.IsPublished;
        entity.DisplayOrder       = vm.DisplayOrder;
        entity.MetaTitleTH        = vm.MetaTitleTH;
        entity.MetaTitleEN        = vm.MetaTitleEN;
        entity.MetaDescriptionTH  = vm.MetaDescriptionTH;
        entity.MetaDescriptionEN  = vm.MetaDescriptionEN;
        entity.IsActive           = vm.IsActive;
        entity.UpdatedAt          = DateTime.UtcNow;
        entity.UpdatedBy          = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Activity updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Details ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var a = await _db.Activities.AsNoTracking()
            .Where(x => x.Id == id && !x.IsDelete)
            .Select(x => new
            {
                x.Id,
                x.ActivitySlug,
                x.ActivityTitleEN,
                x.ActivityTitleTH,
                x.ShortDescriptionEN,
                x.CoverImageUrl,
                x.BannerImageUrl,
                x.ActivityDate,
                x.PublishedDate,
                x.AuthorName,
                x.LocationEN,
                x.IsFeatured,
                x.IsPublished,
                x.IsActive,
                x.DisplayOrder,
                x.ViewCount,
                x.CreatedAt,
                x.CreatedBy,
                x.UpdatedAt,
                x.UpdatedBy,
                TagCount     = x.ActivityTagMaps.Count,
                BlockCount   = x.DetailBlocks.Count(b => !b.IsDelete),
                ImageCount   = x.Images.Count(i => !i.IsDelete),
                RelatedCount = x.RelatedItems.Count,
            })
            .FirstOrDefaultAsync();

        if (a is null) return NotFound();

        ViewData["Title"] = $"Activity Details — {a.ActivityTitleEN}";
        return View(a);
    }

    // ── ManageTags ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> ManageTags(int id)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (activity is null) return NotFound();

        var allTags = await _db.ActivityTags.AsNoTracking()
            .Where(t => !t.IsDelete && t.IsActive)
            .OrderBy(t => t.DisplayOrder)
            .ToListAsync();

        var existingMaps = await _db.ActivityTagMaps.AsNoTracking()
            .Where(m => m.ActivityId == id)
            .ToListAsync();

        var vm = new ActivityTagManageViewModel
        {
            ActivityId      = id,
            ActivityTitleEN = activity.ActivityTitleEN ?? string.Empty,
            PrimaryTagId    = existingMaps.FirstOrDefault(m => m.IsPrimary)?.ActivityTagId,
            Tags = allTags.Select(t => new ActivityTagCheckItem
            {
                TagId      = t.Id,
                TagNameEN  = t.TagNameEN ?? t.TagKey,
                TagColor   = t.TagColor,
                IsSelected = existingMaps.Any(m => m.ActivityTagId == t.Id),
                IsPrimary  = existingMaps.Any(m => m.ActivityTagId == t.Id && m.IsPrimary),
            }).ToList(),
        };

        ViewData["Title"] = $"Manage Tags — {activity.ActivityTitleEN}";
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTags(int id, int[] selectedTagIds, int? primaryTagId)
    {
        var activity = await _db.Activities.FindAsync(id);
        if (activity is null || activity.IsDelete) return NotFound();

        var existingMaps = await _db.ActivityTagMaps
            .Where(m => m.ActivityId == id)
            .ToListAsync();

        // Remove maps not in selectedTagIds
        var toRemove = existingMaps.Where(m => !selectedTagIds.Contains(m.ActivityTagId)).ToList();
        _db.ActivityTagMaps.RemoveRange(toRemove);

        // Add new maps
        var existingTagIds = existingMaps.Select(m => m.ActivityTagId).ToHashSet();
        int order = existingMaps.Count(m => selectedTagIds.Contains(m.ActivityTagId)) + 1;
        foreach (var tagId in selectedTagIds.Where(t => !existingTagIds.Contains(t)))
        {
            _db.ActivityTagMaps.Add(new ActivityTagMap
            {
                ActivityId    = id,
                ActivityTagId = tagId,
                IsPrimary     = tagId == primaryTagId,
                DisplayOrder  = order++,
                CreatedAt     = DateTime.UtcNow,
                CreatedBy     = CurrentUser,
            });
        }

        // Update IsPrimary on existing kept maps
        foreach (var map in existingMaps.Where(m => selectedTagIds.Contains(m.ActivityTagId)))
        {
            map.IsPrimary = map.ActivityTagId == primaryTagId;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Tags updated successfully.";
        return RedirectToAction(nameof(ManageTags), new { id });
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Activities.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Activity deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
