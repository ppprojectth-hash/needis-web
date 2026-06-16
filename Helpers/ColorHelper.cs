using System.Text.RegularExpressions;

namespace Needis.Web.Helpers;

public static partial class ColorHelper
{
    public const string DefaultPrimaryColor = "#2d4199";

    [GeneratedRegex(@"^#(?:[0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")]
    private static partial Regex HexColorRegex();

    public static bool IsValidHexColor(string? value) =>
        !string.IsNullOrWhiteSpace(value) && HexColorRegex().IsMatch(value.Trim());

    public static string NormalizeHexColor(string? value, string fallback = DefaultPrimaryColor) =>
        IsValidHexColor(value) ? value!.Trim() : fallback;
}
