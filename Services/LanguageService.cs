namespace Needis.Web.Services;

public class LanguageService : ILanguageService
{
    public const  string   CookieName        = "Needis.Language";
    public const  string   DefaultLanguage   = "en";
    private static readonly string[] Supported = ["th", "en"];

    public bool IsSupportedLanguage(string? language) =>
        !string.IsNullOrEmpty(language) && Supported.Contains(language);

    public string GetCurrentLanguage(HttpContext context)
    {
        // Query string takes precedence — set cookie and return
        var fromQuery = context.Request.Query["lang"].FirstOrDefault();
        if (IsSupportedLanguage(fromQuery))
        {
            SetLanguage(context, fromQuery!);
            return fromQuery!;
        }

        // Cookie fallback
        var fromCookie = context.Request.Cookies[CookieName];
        return IsSupportedLanguage(fromCookie) ? fromCookie! : DefaultLanguage;
    }

    public void SetLanguage(HttpContext context, string language)
    {
        if (!IsSupportedLanguage(language)) return;

        context.Response.Cookies.Append(CookieName, language, new CookieOptions
        {
            Expires     = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly    = false,
            SameSite    = SameSiteMode.Lax,
            IsEssential = true,
        });
    }

    public string GetLocalizedText(string? th, string? en, string currentLanguage) =>
        currentLanguage == "th" ? (th ?? en ?? "") : (en ?? th ?? "");
}
