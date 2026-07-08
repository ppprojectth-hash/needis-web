using System.Text;
using System.Text.RegularExpressions;

namespace Needis.Web.Helpers;

public static class SlugHelper
{
    private static readonly Regex NonAlphanumericRegex = new(@"[^a-z0-9\-]", RegexOptions.Compiled);
    private static readonly Regex MultiHyphenRegex     = new(@"-{2,}", RegexOptions.Compiled);

    public static string Generate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Guid.NewGuid().ToString("N")[..8];

        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
            if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        var slug = sb.ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant()
            .Trim()
            .Replace(' ', '-');

        slug = NonAlphanumericRegex.Replace(slug, "-");
        slug = MultiHyphenRegex.Replace(slug, "-").Trim('-');

        return string.IsNullOrEmpty(slug) ? Guid.NewGuid().ToString("N")[..8] : slug;
    }
}
