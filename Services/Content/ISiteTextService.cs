namespace Needis.Web.Services.Content;

public interface ISiteTextService
{
    string GetText(string key, string languageCode, string fallback = "");
    Task<string> GetTextAsync(string key, string languageCode, string fallback = "");
    Task<Dictionary<string, string>> GetTextsAsync(IEnumerable<string> keys, string languageCode);
}
