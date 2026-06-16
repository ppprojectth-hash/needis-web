using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services;

namespace Needis.Web.ViewComponents;

public class SiteHeaderViewComponent : ViewComponent
{
    private readonly ISiteSettingService _siteSetting;
    private readonly ILanguageService    _lang;

    public SiteHeaderViewComponent(ISiteSettingService siteSetting, ILanguageService lang)
    {
        _siteSetting = siteSetting;
        _lang        = lang;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var setting = await _siteSetting.GetActiveAsync();

        var currentLang = _lang.GetCurrentLanguage(HttpContext);
        var returnUrl   = HttpContext.Request.Path.Value ?? "/";

        ViewData["CurrentLanguage"] = currentLang;
        ViewData["CurrentPath"]     = returnUrl;
        ViewData["ReturnUrl"]       = returnUrl;

        return View(setting);
    }
}
