using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services;
using Needis.Web.ViewModels.Services;

namespace Needis.Web.Controllers;

public class ServicesController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILanguageService _lang;

    public ServicesController(AppDbContext db, ILanguageService lang)
    {
        _db   = db;
        _lang = lang;
    }

    // ── GET /Services ────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        ViewData["FullWidth"]  = true;
        ViewData["SeoPageKey"] = "services";

        var servicePage = await _db.ServicePages.AsNoTracking()
                              .Where(p => p.IsActive && !p.IsDelete && p.PageKey == "services_main")
                              .FirstOrDefaultAsync()
                          ?? await _db.ServicePages.AsNoTracking()
                              .Where(p => p.IsActive && !p.IsDelete)
                              .OrderBy(p => p.DisplayOrder)
                              .FirstOrDefaultAsync();

        var allServices = await _db.Services.AsNoTracking()
            .Where(s => s.IsActive && !s.IsDelete)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.ServiceNameEN)
            .ToListAsync();

        var vm = new ServicesIndexViewModel
        {
            CurrentLanguage  = lang,
            ServicePage      = servicePage,
            Services         = allServices,
            FeaturedServices = allServices.Where(s => s.IsFeatured).Take(6).ToList(),
        };

        return View(vm);
    }

    // ── GET /Services/Detail/{slug} ──────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Detail(string slug)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        ViewData["FullWidth"] = true;

        var svc = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(s => s.ServiceSlug == slug && s.IsActive && !s.IsDelete);

        if (svc is null) return NotFound();

        var detailSections = await _db.ServiceDetailSections.AsNoTracking()
            .Include(ds => ds.ScopeItems
                .Where(si => si.IsActive && !si.IsDelete)
                .OrderBy(si => si.DisplayOrder).ThenBy(si => si.Id))
            .Where(ds => ds.ServiceId == svc.Id && ds.IsActive && !ds.IsDelete)
            .OrderBy(ds => ds.DisplayOrder)
            .ThenBy(ds => ds.Id)
            .ToListAsync();

        var contactCards = await _db.ServiceContactCards.AsNoTracking()
            .Where(c => c.ServiceId == svc.Id && c.IsActive && !c.IsDelete)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id)
            .ToListAsync();

        var relatedServices = await _db.Services.AsNoTracking()
            .Where(s => s.Id != svc.Id && s.IsActive && !s.IsDelete)
            .OrderByDescending(s => s.IsFeatured)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.ServiceNameEN)
            .Take(3)
            .ToListAsync();

        var vm = new ServiceDetailViewModel
        {
            CurrentLanguage = lang,
            Service         = svc,
            DetailSections  = detailSections,
            ContactCards    = contactCards,
            RelatedServices = relatedServices,
        };

        ViewData["SeoPageKey"]       = "service-detail";
        ViewData["SeoEntityType"]    = "Service";
        ViewData["SeoEntityId"]      = svc.Id;
        ViewData["SeoTitleTH"]       = svc.ServiceNameTH;
        ViewData["SeoTitleEN"]       = svc.ServiceNameEN;
        ViewData["SeoDescriptionTH"] = svc.ShortDescriptionTH;
        ViewData["SeoDescriptionEN"] = svc.ShortDescriptionEN;
        ViewData["SeoImageUrl"]      = svc.BannerImageUrl ?? svc.CoverImageUrl;
        return View(vm);
    }
}
