using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services;

namespace Needis.Web.ViewComponents;

public class SiteFooterViewComponent : ViewComponent
{
    private readonly AppDbContext     _db;
    private readonly ILanguageService _lang;

    public SiteFooterViewComponent(AppDbContext db, ILanguageService lang)
    {
        _db   = db;
        _lang = lang;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var setting = await _db.SiteSettings
            .AsNoTracking()
            .Where(s => s.IsActive)
            .FirstOrDefaultAsync();

        var footerContact = await _db.FooterContacts
            .AsNoTracking()
            .Where(f => f.IsActive)
            .FirstOrDefaultAsync();

        ViewData["CurrentLanguage"] = _lang.GetCurrentLanguage(HttpContext);
        ViewData["FooterContact"]   = footerContact;

        return View(setting);
    }
}
