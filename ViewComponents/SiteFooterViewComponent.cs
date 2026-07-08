using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services;
using Needis.Web.Services.Content;

namespace Needis.Web.ViewComponents;

public class SiteFooterViewComponent : ViewComponent
{
    private readonly AppDbContext        _db;
    private readonly ISiteSettingService _siteSetting;
    private readonly ILanguageService    _lang;
    private readonly ISiteTextService    _siteText;

    private static readonly string[] TextKeys =
    [
        "footer.company.description", "footer.quick_links", "footer.contact", "footer.copyright",
    ];

    public SiteFooterViewComponent(
        AppDbContext db, ISiteSettingService siteSetting, ILanguageService lang, ISiteTextService siteText)
    {
        _db          = db;
        _siteSetting = siteSetting;
        _lang        = lang;
        _siteText    = siteText;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var setting = await _siteSetting.GetActiveAsync();

        var footerContact = await _db.FooterContacts
            .AsNoTracking()
            .Where(f => f.IsActive)
            .FirstOrDefaultAsync();

        var lang = _lang.GetCurrentLanguage(HttpContext);

        ViewData["CurrentLanguage"] = lang;
        ViewData["FooterContact"]   = footerContact;
        ViewData["SiteTexts"]       = await _siteText.GetTextsAsync(TextKeys, lang);

        return View(setting);
    }
}
