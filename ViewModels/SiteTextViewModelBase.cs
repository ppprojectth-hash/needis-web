namespace Needis.Web.ViewModels;

public abstract class SiteTextViewModelBase
{
    public Dictionary<string, string> Texts { get; set; } = new();

    // Returns the edited SiteText value for `key`, or `fallback` if missing/blank.
    public string T(string key, string fallback = "") =>
        Texts.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v) ? v : fallback;
}
