using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services;

namespace Needis.Web.Controllers;

public class LanguageController : Controller
{
    private readonly ILanguageService _lang;

    public LanguageController(ILanguageService lang)
    {
        _lang = lang;
    }

    public IActionResult Change(string lang, string? returnUrl)
    {
        if (_lang.IsSupportedLanguage(lang))
            _lang.SetLanguage(HttpContext, lang);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
