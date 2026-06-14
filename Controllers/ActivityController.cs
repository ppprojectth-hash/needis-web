using DiagActivity = System.Diagnostics.Activity; // prevent conflict with Needis.Web.Models.Activity
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.Services;
using Needis.Web.ViewModels.Activity;

namespace Needis.Web.Controllers;

public class ActivityController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILanguageService _lang;

    public ActivityController(AppDbContext db, ILanguageService lang)
    {
        _db   = db;
        _lang = lang;
    }

    // ── GET /Activity  /Activity?tag=news  /Activity?search=keyword ──────────

    [HttpGet]
    public async Task<IActionResult> Index(string? tag, string? search)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        ViewData["FullWidth"]  = true;
        ViewData["SeoPageKey"] = "activity";

        var now = DateTime.UtcNow;

        // Page header
        var activityPage = await _db.ActivityPages.AsNoTracking()
                               .Where(p => p.IsActive && !p.IsDelete && p.PageKey == "activity_main")
                               .FirstOrDefaultAsync()
                           ?? await _db.ActivityPages.AsNoTracking()
                               .Where(p => p.IsActive && !p.IsDelete)
                               .OrderBy(p => p.DisplayOrder)
                               .FirstOrDefaultAsync();

        // Filterable tags
        var tags = await _db.ActivityTags.AsNoTracking()
            .Where(t => t.IsActive && !t.IsDelete && t.IsFilterable)
            .OrderBy(t => t.DisplayOrder).ThenBy(t => t.TagNameEN)
            .ToListAsync();

        // Base activity query
        var query = _db.Activities.AsNoTracking()
            .Include(a => a.ActivityTagMaps).ThenInclude(m => m.ActivityTag)
            .Where(a => a.IsActive && !a.IsDelete && a.IsPublished &&
                        (a.PublishedDate == null || a.PublishedDate <= now))
            .AsQueryable();

        // Tag filter
        ActivityTag? selectedTag = null;
        if (!string.IsNullOrWhiteSpace(tag))
        {
            selectedTag = await _db.ActivityTags.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TagKey == tag && t.IsActive && !t.IsDelete);

            if (selectedTag is not null)
                query = query.Where(a => a.ActivityTagMaps.Any(m => m.ActivityTagId == selectedTag.Id));
        }

        // Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var kw = search.Trim();
            query = query.Where(a =>
                (a.ActivityTitleTH      != null && a.ActivityTitleTH.Contains(kw))      ||
                (a.ActivityTitleEN      != null && a.ActivityTitleEN.Contains(kw))      ||
                (a.ShortDescriptionTH   != null && a.ShortDescriptionTH.Contains(kw))   ||
                (a.ShortDescriptionEN   != null && a.ShortDescriptionEN.Contains(kw))   ||
                (a.SummaryTH            != null && a.SummaryTH.Contains(kw))            ||
                (a.SummaryEN            != null && a.SummaryEN.Contains(kw))            ||
                (a.AuthorName           != null && a.AuthorName.Contains(kw))           ||
                (a.LocationTH           != null && a.LocationTH.Contains(kw))           ||
                (a.LocationEN           != null && a.LocationEN.Contains(kw)));
        }

        var allActivities = await query
            .OrderByDescending(a => a.IsFeatured)
            .ThenByDescending(a => a.PublishedDate)
            .ThenByDescending(a => a.ActivityDate)
            .ThenBy(a => a.DisplayOrder)
            .Take(50)
            .ToListAsync();

        var featured = string.IsNullOrWhiteSpace(search)
            ? allActivities.Where(a => a.IsFeatured).Take(3).ToList()
            : new List<Models.Activity>();

        var vm = new ActivityIndexViewModel
        {
            CurrentLanguage    = lang,
            ActivityPage       = activityPage,
            Tags               = tags,
            Activities         = allActivities,
            FeaturedActivities = featured,
            SelectedTagKey     = selectedTag?.TagKey,
            SelectedTag        = selectedTag,
            Search             = search,
        };

        return View(vm);
    }

    // ── GET /Activity/Detail/{slug} ───────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Detail(string slug)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        ViewData["FullWidth"] = true;

        var now = DateTime.UtcNow;

        // Load activity with tag maps
        var activity = await _db.Activities.AsNoTracking()
            .Include(a => a.ActivityTagMaps).ThenInclude(m => m.ActivityTag)
            .FirstOrDefaultAsync(a => a.ActivitySlug == slug &&
                                      a.IsActive && !a.IsDelete && a.IsPublished);

        if (activity is null) return NotFound();

        // Collect active tags from maps
        var activityTags = activity.ActivityTagMaps
            .Where(m => m.ActivityTag is { IsActive: true, IsDelete: false })
            .OrderBy(m => m.DisplayOrder)
            .Select(m => m.ActivityTag!)
            .ToList();

        // Detail blocks (with block-level images)
        var detailBlocks = await _db.ActivityDetailBlocks.AsNoTracking()
            .Include(b => b.Images.Where(i => i.IsActive && !i.IsDelete).OrderBy(i => i.DisplayOrder))
            .Where(b => b.ActivityId == activity.Id && b.IsActive && !b.IsDelete)
            .OrderBy(b => b.DisplayOrder).ThenBy(b => b.Id)
            .ToListAsync();

        // Activity-level images not linked to a block
        var freeImages = await _db.ActivityImages.AsNoTracking()
            .Where(i => i.ActivityId == activity.Id && i.IsActive && !i.IsDelete &&
                        i.ActivityDetailBlockId == null)
            .OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id)
            .ToListAsync();

        // Related items (only published, active, non-deleted related activities)
        var relatedItems = await _db.ActivityRelatedItems.AsNoTracking()
            .Include(r => r.RelatedActivity)
            .Where(r => r.ActivityId == activity.Id && r.IsActive)
            .OrderBy(r => r.DisplayOrder)
            .ToListAsync();

        relatedItems = relatedItems
            .Where(r => r.RelatedActivity is { IsActive: true, IsDelete: false, IsPublished: true })
            .ToList();

        // Fallback: more activities if no explicit related items
        var moreActivities = new List<Models.Activity>();
        if (!relatedItems.Any())
        {
            moreActivities = await _db.Activities.AsNoTracking()
                .Include(a => a.ActivityTagMaps).ThenInclude(m => m.ActivityTag)
                .Where(a => a.Id != activity.Id && a.IsActive && !a.IsDelete && a.IsPublished &&
                            (a.PublishedDate == null || a.PublishedDate <= now))
                .OrderByDescending(a => a.IsFeatured)
                .ThenByDescending(a => a.PublishedDate)
                .Take(3)
                .ToListAsync();
        }

        // Increment ViewCount without blocking the page
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var tracked = await db.Activities.FindAsync(activity.Id);
                if (tracked is not null)
                {
                    tracked.ViewCount++;
                    await db.SaveChangesAsync();
                }
            }
            catch { /* silent — do not break page */ }
        });

        var vm = new ActivityDetailViewModel
        {
            CurrentLanguage = lang,
            Activity        = activity,
            Tags            = activityTags,
            DetailBlocks    = detailBlocks,
            Images          = freeImages,
            RelatedItems    = relatedItems,
            MoreActivities  = moreActivities,
        };

        ViewData["SeoPageKey"]       = "activity-detail";
        ViewData["SeoEntityType"]    = "Activity";
        ViewData["SeoEntityId"]      = activity.Id;
        ViewData["SeoTitleTH"]       = activity.MetaTitleTH ?? activity.ActivityTitleTH;
        ViewData["SeoTitleEN"]       = activity.MetaTitleEN ?? activity.ActivityTitleEN;
        ViewData["SeoDescriptionTH"] = activity.MetaDescriptionTH ?? activity.ShortDescriptionTH;
        ViewData["SeoDescriptionEN"] = activity.MetaDescriptionEN ?? activity.ShortDescriptionEN;
        ViewData["SeoImageUrl"]      = activity.BannerImageUrl ?? activity.CoverImageUrl;
        return View(vm);
    }
}
