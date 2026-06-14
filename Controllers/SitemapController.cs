using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using System.Text;
using System.Xml;

namespace Needis.Web.Controllers;

public class SitemapController : Controller
{
    private readonly AppDbContext _db;

    public SitemapController(AppDbContext db) => _db = db;

    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        // Load SEO settings into a lookup keyed by pageKey (static) or entityType+entityId (dynamic)
        var seoSettings = await _db.SeoSettings.AsNoTracking()
            .Where(s => s.IsActive && !s.IsDelete)
            .ToListAsync();

        var seoByPageKey    = seoSettings
            .Where(s => s.EntityId == null)
            .GroupBy(s => s.PageKey)
            .ToDictionary(g => g.Key, g => g.First());

        var seoByEntity     = seoSettings
            .Where(s => s.EntityType != null && s.EntityId.HasValue)
            .ToDictionary(s => (s.EntityType!, s.EntityId!.Value));

        var entries = new List<SitemapEntry>();
        var now     = DateTime.UtcNow;

        // ── Static pages ────────────────────────────────────────────────────
        AddStatic(entries, baseUrl, "/",          "home",     seoByPageKey, "daily",   1.0m, now);
        AddStatic(entries, baseUrl, "/About",     "about",    seoByPageKey, "monthly", 0.7m, now);
        AddStatic(entries, baseUrl, "/Product",   "products", seoByPageKey, "weekly",  0.8m, now);
        AddStatic(entries, baseUrl, "/Services",  "services", seoByPageKey, "weekly",  0.8m, now);
        AddStatic(entries, baseUrl, "/Activity",  "activity", seoByPageKey, "weekly",  0.8m, now);
        AddStatic(entries, baseUrl, "/Contact",   "contact",  seoByPageKey, "monthly", 0.5m, now);

        // ── Product categories ──────────────────────────────────────────────
        var categories = await _db.ProductCategories.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEN)
            .ToListAsync();

        foreach (var cat in categories)
        {
            var url   = $"{baseUrl}/Product?category={cat.Slug}";
            seoByEntity.TryGetValue(("ProductCategory", cat.Id), out var catSeo);
            if (catSeo is { IncludeInSitemap: false }) continue;

            entries.Add(new SitemapEntry
            {
                Loc        = url,
                LastMod    = cat.UpdatedAt,
                ChangeFreq = catSeo?.ChangeFrequency ?? "weekly",
                Priority   = catSeo?.Priority        ?? 0.7m,
            });
        }

        // ── Products ────────────────────────────────────────────────────────
        var products = await _db.Products.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.NameEN)
            .ToListAsync();

        foreach (var product in products)
        {
            seoByEntity.TryGetValue(("Product", product.Id), out var prodSeo);
            if (prodSeo is { IncludeInSitemap: false }) continue;

            entries.Add(new SitemapEntry
            {
                Loc        = $"{baseUrl}/Product/Detail/{product.Slug}",
                LastMod    = product.UpdatedAt,
                ChangeFreq = prodSeo?.ChangeFrequency ?? "weekly",
                Priority   = prodSeo?.Priority        ?? 0.7m,
            });
        }

        // ── Services ────────────────────────────────────────────────────────
        var services = await _db.Services.AsNoTracking()
            .Where(s => s.IsActive && !s.IsDelete)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.ServiceNameEN)
            .ToListAsync();

        foreach (var svc in services)
        {
            seoByEntity.TryGetValue(("Service", svc.Id), out var svcSeo);
            if (svcSeo is { IncludeInSitemap: false }) continue;

            entries.Add(new SitemapEntry
            {
                Loc        = $"{baseUrl}/Services/Detail/{svc.ServiceSlug}",
                LastMod    = svc.UpdatedAt ?? svc.CreatedAt,
                ChangeFreq = svcSeo?.ChangeFrequency ?? "weekly",
                Priority   = svcSeo?.Priority        ?? 0.7m,
            });
        }

        // ── Activities ──────────────────────────────────────────────────────
        var activities = await _db.Activities.AsNoTracking()
            .Where(a => a.IsActive && !a.IsDelete && a.IsPublished)
            .OrderByDescending(a => a.PublishedDate)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

        foreach (var act in activities)
        {
            seoByEntity.TryGetValue(("Activity", act.Id), out var actSeo);
            if (actSeo is { IncludeInSitemap: false }) continue;

            entries.Add(new SitemapEntry
            {
                Loc        = $"{baseUrl}/Activity/Detail/{act.ActivitySlug}",
                LastMod    = act.UpdatedAt ?? act.CreatedAt,
                ChangeFreq = actSeo?.ChangeFrequency ?? "weekly",
                Priority   = actSeo?.Priority        ?? 0.7m,
            });
        }

        return Content(BuildXml(entries), "application/xml", Encoding.UTF8);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static void AddStatic(
        List<SitemapEntry>                  entries,
        string                              baseUrl,
        string                              path,
        string                              pageKey,
        Dictionary<string, SeoSetting>     seoByPageKey,
        string                              defaultFreq,
        decimal                             defaultPriority,
        DateTime                            fallbackDate)
    {
        seoByPageKey.TryGetValue(pageKey, out var seo);
        if (seo is { IncludeInSitemap: false }) return;

        entries.Add(new SitemapEntry
        {
            Loc        = baseUrl + path,
            LastMod    = seo?.UpdatedAt ?? seo?.CreatedAt ?? fallbackDate,
            ChangeFreq = seo?.ChangeFrequency ?? defaultFreq,
            Priority   = seo?.Priority        ?? defaultPriority,
        });
    }

    private static string BuildXml(IEnumerable<SitemapEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        foreach (var e in entries)
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{XmlEncode(e.Loc)}</loc>");
            sb.AppendLine($"    <lastmod>{e.LastMod:yyyy-MM-dd}</lastmod>");
            sb.AppendLine($"    <changefreq>{e.ChangeFreq}</changefreq>");
            sb.AppendLine($"    <priority>{e.Priority:0.0}</priority>");
            sb.AppendLine("  </url>");
        }

        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    private static string XmlEncode(string s) => s
        .Replace("&",  "&amp;")
        .Replace("<",  "&lt;")
        .Replace(">",  "&gt;")
        .Replace("\"", "&quot;")
        .Replace("'",  "&apos;");

    private sealed class SitemapEntry
    {
        public string   Loc        { get; set; } = string.Empty;
        public DateTime LastMod    { get; set; }
        public string   ChangeFreq { get; set; } = "weekly";
        public decimal  Priority   { get; set; } = 0.8m;
    }
}
