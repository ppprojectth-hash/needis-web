using Needis.Web.ViewModels.Seo;

namespace Needis.Web.Services.Seo;

public interface ISeoService
{
    Task<SeoViewModel> GetSeoAsync(
        HttpContext context,
        string      pageKey,
        string?     entityType            = null,
        int?        entityId              = null,
        string?     fallbackTitleTH       = null,
        string?     fallbackTitleEN       = null,
        string?     fallbackDescriptionTH = null,
        string?     fallbackDescriptionEN = null,
        string?     fallbackImageUrl      = null);

    string BuildCanonicalUrl(HttpContext context, string? path = null);
}
