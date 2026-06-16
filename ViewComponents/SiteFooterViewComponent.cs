using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services;

namespace Needis.Web.ViewComponents;

public class SiteFooterViewComponent : ViewComponent
{
    private readonly AppDbContext        _db;
    private readonly ISiteSettingService _siteSetting;
    private readonly ILanguageService    _lang;

    public SiteFooterViewComponent(AppDbContext db, ISiteSettingService siteSetting, ILanguageService lang)
    {
        _db          = db;
        _siteSetting = siteSetting;
        _lang        = lang;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var setting = await _siteSetting.GetActiveAsync();

        var footerContact = await _db.FooterContacts
            .AsNoTracking()
            .Where(f => f.IsActive)
            .FirstOrDefaultAsync();

        ViewData["CurrentLanguage"] = _lang.GetCurrentLanguage(HttpContext);
        ViewData["FooterContact"]   = footerContact;

        return View(setting);
    }
}
