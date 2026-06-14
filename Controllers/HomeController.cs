using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.Services;
using Needis.Web.ViewModels.Home;
using DiagActivity = System.Diagnostics.Activity;

namespace Needis.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext     _db;
    private readonly ILanguageService _lang;

    public HomeController(AppDbContext db, ILanguageService lang)
    {
        _db   = db;
        _lang = lang;
    }

    public async Task<IActionResult> Index()
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        var siteSetting = await _db.SiteSettings
            .AsNoTracking()
            .Where(s => s.IsActive)
            .FirstOrDefaultAsync();

        var now = DateTime.UtcNow;
        var allBanners = await _db.HomeBanners
            .AsNoTracking()
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();

        var banners = allBanners
            .Where(b =>
                (b.StartDate == null || b.StartDate.Value <= now.AddDays(1)) &&
                (b.EndDate   == null || b.EndDate.Value.AddDays(1) >= now))
            .ToList();

        var categories = await _db.ProductCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEN)
            .ToListAsync();

        // Load enough featured products to fill up to 4 category groups × 3 products
        var featuredProducts = await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.NameEN)
            .Take(24)
            .ToListAsync();

        var featuredServices = await _db.Services
            .AsNoTracking()
            .Where(s => s.IsActive && !s.IsDelete)
            .OrderByDescending(s => s.IsFeatured)
            .ThenBy(s => s.DisplayOrder)
            .Take(4)
            .ToListAsync();

        var latestActivities = await _db.Activities
            .AsNoTracking()
            .Where(a => a.IsPublished && a.IsActive && !a.IsDelete)
            .OrderByDescending(a => a.DisplayOrder)
            .ThenByDescending(a => a.CreatedAt)
            .Take(3)
            .ToListAsync();

        // Build product groups: categories that have featured products, up to 4 groups
        var productGroups = categories
            .Select(cat => new HomeCategoryProductGroupViewModel
            {
                CategoryId            = cat.Id,
                CategoryNameTH        = cat.NameTH,
                CategoryNameEN        = cat.NameEN,
                CategoryDescriptionTH = cat.ShortDescriptionTH,
                CategoryDescriptionEN = cat.ShortDescriptionEN,
                CategorySlug          = cat.Slug,
                CategoryImagePath     = cat.ImagePath,
                Products              = featuredProducts
                    .Where(p => p.CategoryId == cat.Id)
                    .Take(3)
                    .ToList()
            })
            .Where(g => g.Products.Any())
            .Take(4)
            .ToList();

        var vm = new HomeIndexViewModel
        {
            SiteSetting      = siteSetting,
            Banners          = banners,
            Categories       = categories,
            FeaturedProducts = featuredProducts,
            ProductGroups    = productGroups,
            FeaturedServices = featuredServices,
            LatestActivities = latestActivities,
            CurrentLanguage  = lang,
        };

        ViewData["SeoPageKey"] = "home";
        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = DiagActivity.Current?.Id ?? HttpContext.TraceIdentifier });
}
