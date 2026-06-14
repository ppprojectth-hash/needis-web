using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services;

namespace Needis.Web.ViewComponents;

public class SiteHeaderViewComponent : ViewComponent
{
    private readonly AppDbContext     _db;
    private readonly ILanguageService _lang;

    public SiteHeaderViewComponent(AppDbContext db, ILanguageService lang)
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

        var currentLang = _lang.GetCurrentLanguage(HttpContext);
        var returnUrl   = HttpContext.Request.Path.Value ?? "/";

        ViewData["CurrentLanguage"] = currentLang;
        ViewData["CurrentPath"]     = returnUrl;
        ViewData["ReturnUrl"]       = returnUrl;

        return View(setting);
    }
}
