using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Seo;

namespace Needis.Web.Services.Seo;

public class SeoService : ISeoService
{
    private readonly AppDbContext    _db;
    private readonly ILanguageService _lang;

    public SeoService(AppDbContext db, ILanguageService lang)
    {
        _db   = db;
        _lang = lang;
    }

    public async Task<SeoViewModel> GetSeoAsync(
        HttpContext context,
        string      pageKey,
        string?     entityType            = null,
        int?        entityId              = null,
        string?     fallbackTitleTH       = null,
        string?     fallbackTitleEN       = null,
        string?     fallbackDescriptionTH = null,
        string?     fallbackDescriptionEN = null,
        string?     fallbackImageUrl      = null)
    {
        var lang = _lang.GetCurrentLanguage(context);

        SeoSetting? setting = null;

        // 1. Try EntityType + EntityId match first
        if (entityType != null && entityId.HasValue)
        {
            setting = await _db.SeoSettings.AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.EntityType == entityType &&
                    s.EntityId   == entityId.Value &&
                    s.IsActive   && !s.IsDelete);
        }

        // 2. Fall back to PageKey match
        if (setting is null)
        {
            setting = await _db.SeoSettings.AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.PageKey == pageKey &&
                    s.EntityId == null  &&
                    s.IsActive && !s.IsDelete);
        }

        // Resolve localized values with TH→EN and EN→TH fallbacks
        string? title       = Localize(setting?.MetaTitleTH,       setting?.MetaTitleEN,       fallbackTitleTH,       fallbackTitleEN,       lang);
        string? description = Localize(setting?.MetaDescriptionTH, setting?.MetaDescriptionEN, fallbackDescriptionTH, fallbackDescriptionEN, lang);
        string? keywords    = lang == "th" ? (setting?.MetaKeywordsTH ?? setting?.MetaKeywordsEN)
                                           : (setting?.MetaKeywordsEN ?? setting?.MetaKeywordsTH);
        string? ogTitle     = Localize(setting?.OgTitleTH,       setting?.OgTitleEN,       fallbackTitleTH,       fallbackTitleEN,       lang);
        string? ogDesc      = Localize(setting?.OgDescriptionTH, setting?.OgDescriptionEN, fallbackDescriptionTH, fallbackDescriptionEN, lang);
        string? ogImage     = setting?.OgImageUrl ?? fallbackImageUrl;
        string? canonical   = string.IsNullOrWhiteSpace(setting?.CanonicalUrl)
            ? BuildCanonicalUrl(context)
            : setting.CanonicalUrl;
        string? robots      = setting?.Robots ?? "index, follow";

        return new SeoViewModel
        {
            Title           = title,
            Description     = description,
            Keywords        = keywords,
            OgTitle         = ogTitle ?? title,
            OgDescription   = ogDesc  ?? description,
            OgImageUrl      = ogImage,
            CanonicalUrl    = canonical,
            Robots          = robots,
            CurrentLanguage = lang,
        };
    }

    public string BuildCanonicalUrl(HttpContext context, string? path = null)
    {
        var request = context.Request;
        var scheme  = request.Scheme;
        var host    = request.Host.ToUriComponent();
        var p       = path ?? (request.Path.ToString() + request.QueryString.ToString());
        return $"{scheme}://{host}{p}";
    }

    // Returns the localized string with fallbacks: preferred lang → other lang → fallback preferred → fallback other
    private static string? Localize(
        string? th, string? en,
        string? fallbackTh, string? fallbackEn,
        string lang)
    {
        if (lang == "th")
            return th ?? en ?? fallbackTh ?? fallbackEn;
        return en ?? th ?? fallbackEn ?? fallbackTh;
    }
}
