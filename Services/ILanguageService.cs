namespace Needis.Web.Services;

public interface ILanguageService
{
    string GetCurrentLanguage(HttpContext context);
    void   SetLanguage(HttpContext context, string language);
    bool   IsSupportedLanguage(string? language);
    string GetLocalizedText(string? th, string? en, string currentLanguage);
}
