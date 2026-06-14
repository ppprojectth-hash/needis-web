using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services.Seo;

namespace Needis.Web.ViewComponents;

public class SeoMetaViewComponent : ViewComponent
{
    private readonly ISeoService _seo;

    public SeoMetaViewComponent(ISeoService seo)
    {
        _seo = seo;
    }

    public async Task<IViewComponentResult> InvokeAsync(
        string  pageKey              = "home",
        string? entityType           = null,
        int?    entityId             = null,
        string? fallbackTitleTH      = null,
        string? fallbackTitleEN      = null,
        string? fallbackDescriptionTH = null,
        string? fallbackDescriptionEN = null,
        string? fallbackImageUrl     = null)
    {
        var vm = await _seo.GetSeoAsync(
            HttpContext,
            pageKey,
            entityType,
            entityId,
            fallbackTitleTH,
            fallbackTitleEN,
            fallbackDescriptionTH,
            fallbackDescriptionEN,
            fallbackImageUrl);

        return View(vm);
    }
}
