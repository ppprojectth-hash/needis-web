using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SeoSettingController : Controller
{
    private readonly AppDbContext _db;

    public SeoSettingController(AppDbContext db) => _db = db;

    private static readonly string[] ValidFrequencies =
        ["always", "hourly", "daily", "weekly", "monthly", "yearly", "never"];

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Seo.View")]
    public async Task<IActionResult> Index(string? keyword, string? entityType)
    {
        var query = _db.SeoSettings.AsNoTracking().Where(s => !s.IsDelete);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(s =>
                s.PageKey.Contains(kw) ||
                (s.RoutePath    != null && s.RoutePath.Contains(kw))    ||
                (s.MetaTitleEN  != null && s.MetaTitleEN.Contains(kw))  ||
                (s.MetaTitleTH  != null && s.MetaTitleTH.Contains(kw))  ||
                (s.EntityType   != null && s.EntityType.Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(s => s.EntityType == entityType);

        var items = await query
            .OrderBy(s => s.PageKey)
            .ThenBy(s => s.EntityId)
            .Select(s => new SeoSettingListItemViewModel
            {
                Id               = s.Id,
                PageKey          = s.PageKey,
                EntityType       = s.EntityType,
                EntityId         = s.EntityId,
                RoutePath        = s.RoutePath,
                MetaTitleEN      = s.MetaTitleEN,
                MetaTitleTH      = s.MetaTitleTH,
                IncludeInSitemap = s.IncludeInSitemap,
                IsActive         = s.IsActive,
                UpdatedAt        = s.UpdatedAt,
            })
            .ToListAsync();

        var entityTypes = await _db.SeoSettings.AsNoTracking()
            .Where(s => !s.IsDelete && s.EntityType != null)
            .Select(s => s.EntityType!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        ViewBag.Keyword     = keyword;
        ViewBag.EntityType  = entityType;
        ViewBag.EntityTypes = entityTypes;
        return View(items);
    }

    // ── Create GET ───────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Seo.Edit")]
    public IActionResult Create()
    {
        ViewBag.Frequencies = ValidFrequencies;
        return View(new SeoSettingEditViewModel());
    }

    // ── Create POST ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Create(SeoSettingEditViewModel vm)
    {
        ValidateFrequency(vm.ChangeFrequency);
        if (!ModelState.IsValid) { ViewBag.Frequencies = ValidFrequencies; return View(vm); }

        var entry = new SeoSetting
        {
            PageKey          = vm.PageKey.Trim(),
            EntityType       = vm.EntityType?.Trim(),
            EntityId         = vm.EntityId,
            RoutePath        = vm.RoutePath?.Trim(),
            MetaTitleTH      = vm.MetaTitleTH,
            MetaTitleEN      = vm.MetaTitleEN,
            MetaDescriptionTH = vm.MetaDescriptionTH,
            MetaDescriptionEN = vm.MetaDescriptionEN,
            MetaKeywordsTH   = vm.MetaKeywordsTH,
            MetaKeywordsEN   = vm.MetaKeywordsEN,
            OgTitleTH        = vm.OgTitleTH,
            OgTitleEN        = vm.OgTitleEN,
            OgDescriptionTH  = vm.OgDescriptionTH,
            OgDescriptionEN  = vm.OgDescriptionEN,
            OgImageUrl       = vm.OgImageUrl?.Trim(),
            CanonicalUrl     = vm.CanonicalUrl?.Trim(),
            Robots           = string.IsNullOrWhiteSpace(vm.Robots) ? "index, follow" : vm.Robots.Trim(),
            Priority         = vm.Priority,
            ChangeFrequency  = vm.ChangeFrequency,
            IncludeInSitemap = vm.IncludeInSitemap,
            IsActive         = vm.IsActive,
            CreatedAt        = DateTime.UtcNow,
            CreatedBy        = User.Identity?.Name,
        };

        _db.SeoSettings.Add(entry);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"SEO setting for '{entry.PageKey}' created.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit GET ─────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var s = await _db.SeoSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (s is null) return NotFound();

        ViewBag.Frequencies = ValidFrequencies;
        return View(MapToViewModel(s));
    }

    // ── Edit POST ────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Edit(int id, SeoSettingEditViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        ValidateFrequency(vm.ChangeFrequency);
        if (!ModelState.IsValid) { ViewBag.Frequencies = ValidFrequencies; return View(vm); }

        var entry = await _db.SeoSettings.FindAsync(id);
        if (entry is null || entry.IsDelete) return NotFound();

        entry.PageKey          = vm.PageKey.Trim();
        entry.EntityType       = vm.EntityType?.Trim();
        entry.EntityId         = vm.EntityId;
        entry.RoutePath        = vm.RoutePath?.Trim();
        entry.MetaTitleTH      = vm.MetaTitleTH;
        entry.MetaTitleEN      = vm.MetaTitleEN;
        entry.MetaDescriptionTH = vm.MetaDescriptionTH;
        entry.MetaDescriptionEN = vm.MetaDescriptionEN;
        entry.MetaKeywordsTH   = vm.MetaKeywordsTH;
        entry.MetaKeywordsEN   = vm.MetaKeywordsEN;
        entry.OgTitleTH        = vm.OgTitleTH;
        entry.OgTitleEN        = vm.OgTitleEN;
        entry.OgDescriptionTH  = vm.OgDescriptionTH;
        entry.OgDescriptionEN  = vm.OgDescriptionEN;
        entry.OgImageUrl       = vm.OgImageUrl?.Trim();
        entry.CanonicalUrl     = vm.CanonicalUrl?.Trim();
        entry.Robots           = string.IsNullOrWhiteSpace(vm.Robots) ? "index, follow" : vm.Robots.Trim();
        entry.Priority         = vm.Priority;
        entry.ChangeFrequency  = vm.ChangeFrequency;
        entry.IncludeInSitemap = vm.IncludeInSitemap;
        entry.IsActive         = vm.IsActive;
        entry.UpdatedAt        = DateTime.UtcNow;
        entry.UpdatedBy        = User.Identity?.Name;

        await _db.SaveChangesAsync();

        TempData["Success"] = $"SEO setting for '{entry.PageKey}' updated.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete POST ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _db.SeoSettings.FindAsync(id);
        if (entry is not null && !entry.IsDelete)
        {
            entry.IsDelete  = true;
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = User.Identity?.Name;
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "SEO setting deleted.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void ValidateFrequency(string freq)
    {
        if (!ValidFrequencies.Contains(freq))
            ModelState.AddModelError(nameof(SeoSettingEditViewModel.ChangeFrequency),
                $"Invalid frequency. Allowed: {string.Join(", ", ValidFrequencies)}");
    }

    private static SeoSettingEditViewModel MapToViewModel(SeoSetting s) => new()
    {
        Id               = s.Id,
        PageKey          = s.PageKey,
        EntityType       = s.EntityType,
        EntityId         = s.EntityId,
        RoutePath        = s.RoutePath,
        MetaTitleTH      = s.MetaTitleTH,
        MetaTitleEN      = s.MetaTitleEN,
        MetaDescriptionTH = s.MetaDescriptionTH,
        MetaDescriptionEN = s.MetaDescriptionEN,
        MetaKeywordsTH   = s.MetaKeywordsTH,
        MetaKeywordsEN   = s.MetaKeywordsEN,
        OgTitleTH        = s.OgTitleTH,
        OgTitleEN        = s.OgTitleEN,
        OgDescriptionTH  = s.OgDescriptionTH,
        OgDescriptionEN  = s.OgDescriptionEN,
        OgImageUrl       = s.OgImageUrl,
        CanonicalUrl     = s.CanonicalUrl,
        Robots           = s.Robots,
        Priority         = s.Priority,
        ChangeFrequency  = s.ChangeFrequency,
        IncludeInSitemap = s.IncludeInSitemap,
        IsActive         = s.IsActive,
    };
}
