using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services;
using Needis.Web.Services.HomePopup;

namespace Needis.Web.ViewComponents;

public class HomePopupViewComponent : ViewComponent
{
    private readonly IHomePopupService _popupService;
    private readonly ILanguageService  _lang;

    public HomePopupViewComponent(IHomePopupService popupService, ILanguageService lang)
    {
        _popupService = popupService;
        _lang         = lang;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var popup = await _popupService.GetActivePopupAsync();
            if (popup is null) return Content(string.Empty);

            ViewData["CurrentLanguage"] = _lang.GetCurrentLanguage(HttpContext);
            return View(popup);
        }
        catch
        {
            return Content(string.Empty);
        }
    }
}
