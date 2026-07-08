using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;

namespace Needis.Web.Services.Content;

// Caches all active site texts for the lifetime of the scoped DbContext (one request),
// so a page with many keys costs a single query instead of one per key.
public class SiteTextService : ISiteTextService
{
    private readonly AppDbContext _db;
    private Dictionary<string, SiteText>? _cache;

    public SiteTextService(AppDbContext db)
    {
        _db = db;
    }

    public string GetText(string key, string languageCode, string fallback = "")
    {
        var cache = LoadCache();
        return Resolve(cache, key, languageCode, fallback);
    }

    public async Task<string> GetTextAsync(string key, string languageCode, string fallback = "")
    {
        var cache = await LoadCacheAsync();
        return Resolve(cache, key, languageCode, fallback);
    }

    public async Task<Dictionary<string, string>> GetTextsAsync(IEnumerable<string> keys, string languageCode)
    {
        var cache = await LoadCacheAsync();
        var result = new Dictionary<string, string>();
        foreach (var key in keys)
            result[key] = Resolve(cache, key, languageCode, "");
        return result;
    }

    private Dictionary<string, SiteText> LoadCache()
    {
        return _cache ??= _db.SiteTexts
            .AsNoTracking()
            .Where(t => t.IsActive && !t.IsDelete)
            .ToDictionary(t => t.Key);
    }

    private async Task<Dictionary<string, SiteText>> LoadCacheAsync()
    {
        return _cache ??= await _db.SiteTexts
            .AsNoTracking()
            .Where(t => t.IsActive && !t.IsDelete)
            .ToDictionaryAsync(t => t.Key);
    }

    private static string Resolve(
        Dictionary<string, SiteText> cache, string key, string languageCode, string fallback)
    {
        if (!cache.TryGetValue(key, out var text)) return fallback;

        var primary   = languageCode == "th" ? text.TextTH : text.TextEN;
        var secondary = languageCode == "th" ? text.TextEN : text.TextTH;

        if (!string.IsNullOrWhiteSpace(primary))   return primary;
        if (!string.IsNullOrWhiteSpace(secondary)) return secondary;
        return fallback;
    }
}
