using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services;
using Needis.Web.ViewModels.About;

namespace Needis.Web.Controllers;

public class AboutController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILanguageService _lang;

    public AboutController(AppDbContext db, ILanguageService lang)
    {
        _db   = db;
        _lang = lang;
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

        var staff = await _db.StaffProfiles.AsNoTracking()
            .Where(s => s.IsActive && !s.IsDelete && s.ShowPublic)
            .OrderBy(s => s.FullNameEN)
            .Take(8)
            .ToListAsync();

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

        var vm = new AboutIndexViewModel
        {
            CurrentLanguage     = lang,
            Sections            = sections,
            Histories           = histories,
            StatCards           = displayStats,
            BrandPartners       = brandPartners,
            PublicStaffProfiles = staff,
        };

        return View(vm);
    }
}
