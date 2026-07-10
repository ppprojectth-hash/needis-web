using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.Services;
using Needis.Web.Services.Content;
using Needis.Web.Services.Features;
using Needis.Web.ViewModels.About;

namespace Needis.Web.Controllers;

public class AboutController : Controller
{
    private readonly AppDbContext          _db;
    private readonly ILanguageService      _lang;
    private readonly IFeatureFlagService   _features;
    private readonly ISiteTextService      _siteText;
    private readonly ILogger<AboutController> _logger;

    private static readonly string[] TextKeys =
    [
        "about.page.title", "about.page.subtitle",
        "about.company.title", "about.company.description",
        "about.team.eyebrow", "about.team.title", "about.team.subtitle",
        "about.location.title", "about.location.subtitle", "about.location.button",
    ];

    public AboutController(
        AppDbContext db, ILanguageService lang, IFeatureFlagService features, ISiteTextService siteText,
        ILogger<AboutController> logger)
    {
        _db       = db;
        _lang     = lang;
        _features = features;
        _siteText = siteText;
        _logger   = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        ViewData["FullWidth"]  = true;
        ViewData["SeoPageKey"] = "about";

        // Load data sequentially — EF Core DbContext is not thread-safe
        var sections = await _db.AboutSections.AsNoTracking()
            .Where(s => s.IsActive && !s.IsDelete)
            .Include(s => s.Items
                .Where(i => i.IsActive && !i.IsDelete)
                .OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id))
            .OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id)
            .ToListAsync();

        var histories = await _db.AboutCompanyHistories.AsNoTracking()
            .Where(h => h.IsActive && !h.IsDelete)
            .OrderBy(h => h.DisplayOrder).ThenBy(h => h.Year)
            .ToListAsync();

        var rawStatCards = await _db.AboutStatCards.AsNoTracking()
            .Where(c => c.IsActive && !c.IsDelete)
            .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id)
            .ToListAsync();

        var brandPartners = await _db.BrandPartners.AsNoTracking()
            .Where(b => b.IsActive && !b.IsDelete && b.ShowOnPartnerSection)
            .OrderBy(b => b.DisplayOrder).ThenBy(b => b.BrandName)
            .ToListAsync();

        // Public staff card visibility: Active + ShowPublic + within the optional
        // StartDate/EndDate display window. IsExpert is intentionally NOT part of
        // this filter — it only affects the "expert count" stat card below.
        var today = DateTime.UtcNow.Date;
        var staff = await _db.StaffProfiles.AsNoTracking()
            .Where(s => s.IsActive && !s.IsDelete && s.ShowPublic)
            .Where(s => s.StartDate == null || s.StartDate.Value.Date <= today)
            .Where(s => s.EndDate   == null || s.EndDate.Value.Date   >= today)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.FullNameEN)
            .Take(8)
            .ToListAsync();

        var siteSetting = await _db.SiteSettings.AsNoTracking().FirstOrDefaultAsync();

        // Pre-compute dynamic stat values
        var brandCount  = await _db.BrandPartners
            .CountAsync(b => b.IsActive && !b.IsDelete && b.IsGlobalBrand);
        var expertCount = await _db.StaffProfiles
            .CountAsync(s => s.IsActive && !s.IsDelete && s.IsExpert);
        var productSold = await _db.ProductSales
            .Where(ps => ps.IsActive && !ps.IsDelete && ps.CountInAboutStats)
            .SumAsync(ps => (int?)ps.Quantity) ?? 0;

        // Build display stat cards with computed values
        var displayStats = rawStatCards.Select(c =>
        {
            int value = c.SourceType switch
            {
                "GlobalBrandCount"  => brandCount,
                "ExpertStaffCount"  => expertCount,
                "ProductSoldCount"  => productSold,
                _                   => c.ManualValue ?? 0,
            };

            var label = lang == "th"
                ? (c.LabelTH ?? c.LabelEN ?? string.Empty)
                : (c.LabelEN ?? c.LabelTH ?? string.Empty);

            var description = lang == "th"
                ? (c.DescriptionTH ?? c.DescriptionEN)
                : (c.DescriptionEN ?? c.DescriptionTH);

            var displayValue = $"{c.Prefix}{value}{c.Suffix}";

            return new AboutStatCardDisplayViewModel
            {
                Id             = c.Id,
                StatKey        = c.StatKey,
                Label          = label,
                Description    = description,
                Prefix         = c.Prefix,
                Suffix         = c.Suffix,
                IconUrl        = c.IconUrl,
                CardStyle      = c.CardStyle,
                DisplayOrder   = c.DisplayOrder,
                CalculatedValue = value,
                DisplayValue   = displayValue,
            };
        }).ToList();

        // Resolve safe map URLs
        var safeMapUrl   = GoogleMapHelper.IsSafeGoogleMapUrl(siteSetting?.GoogleMapUrl)
                               ? siteSetting!.GoogleMapUrl
                               : null;
        var safeEmbedUrl = GoogleMapHelper.IsSafeGoogleMapEmbedUrl(siteSetting?.GoogleMapEmbedUrl)
                               ? siteSetting!.GoogleMapEmbedUrl
                               : null;

        var texts = await _siteText.GetTextsAsync(TextKeys, lang);

        var vm = new AboutIndexViewModel
        {
            CurrentLanguage     = lang,
            Texts               = texts,
            Sections            = sections,
            Histories           = histories,
            StatCards           = displayStats,
            BrandPartners       = brandPartners,
            PublicStaffProfiles = staff ?? new List<Models.StaffProfile>(),

            ShowMapOnAboutPage  = siteSetting?.ShowMapOnAboutPage ?? false,
            GoogleMapUrl        = safeMapUrl,
            GoogleMapEmbedUrl   = safeEmbedUrl,
            MapTitleTH          = siteSetting?.MapTitleTH,
            MapTitleEN          = siteSetting?.MapTitleEN,
            MapDescriptionTH    = siteSetting?.MapDescriptionTH,
            MapDescriptionEN    = siteSetting?.MapDescriptionEN,
            AddressTH           = siteSetting?.AddressTH,
            AddressEN           = siteSetting?.AddressEN,
            ContactPhone        = siteSetting?.ContactPhone,
            ContactEmail        = siteSetting?.ContactEmail,
        };

        _logger.LogInformation("About page staff profiles loaded: {Count}", vm.PublicStaffProfiles.Count);

        return View(vm);
    }

    [HttpGet("About/Staff/{slug}")]
    public async Task<IActionResult> StaffDetail(string slug)
    {
        if (!_features.StaffProfileDetailEnabled) return NotFound();
        if (string.IsNullOrWhiteSpace(slug)) return NotFound();

        var lang = _lang.GetCurrentLanguage(HttpContext);
        ViewData["SeoPageKey"] = "about-staff";

        var staff = await _db.StaffProfiles.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == slug && s.IsActive && !s.IsDelete);

        if (staff is null || !staff.ShowDetailPage) return NotFound();

        var siteSetting = await _db.SiteSettings.AsNoTracking().FirstOrDefaultAsync();

        ViewBag.CurrentLanguage = lang;
        ViewBag.CompanyName = lang == "th"
            ? (siteSetting?.CompanyNameTH ?? siteSetting?.CompanyNameEN ?? "Needis")
            : (siteSetting?.CompanyNameEN ?? siteSetting?.CompanyNameTH ?? "Needis");

        return View(staff);
    }
}
