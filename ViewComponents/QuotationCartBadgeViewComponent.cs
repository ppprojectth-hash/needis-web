using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services;
using Needis.Web.Services.Quotation;

namespace Needis.Web.ViewComponents;

public class QuotationCartBadgeViewComponent : ViewComponent
{
    private readonly IQuotationCartService _cartService;
    private readonly ILanguageService _lang;

    public QuotationCartBadgeViewComponent(IQuotationCartService cartService, ILanguageService lang)
    {
        _cartService = cartService;
        _lang        = lang;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var count = await _cartService.GetCartItemCountAsync(HttpContext);
        var lang  = _lang.GetCurrentLanguage(HttpContext);
        ViewData["CartCount"]       = count;
        ViewData["CurrentLanguage"] = lang;
        return View(count);
    }
}
