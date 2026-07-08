using System.Text.RegularExpressions;

namespace Needis.Web.Helpers;

public static class YoutubeHelper
{
    // Matches the 11-char video ID in any supported YouTube URL format
    private static readonly Regex VideoIdRegex = new(
        @"(?:youtu\.be/|youtube\.com/(?:watch\?(?:.*&)?v=|embed/|shorts/))([A-Za-z0-9_\-]{11})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string? ExtractVideoId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var m = VideoIdRegex.Match(url.Trim());
        return m.Success ? m.Groups[1].Value : null;
    }

    public static bool IsValidYoutubeUrl(string? url) => ExtractVideoId(url) is not null;

    public static string? GetEmbedUrl(string? url)
    {
        var id = ExtractVideoId(url);
        return id is null ? null : $"https://www.youtube.com/embed/{id}";
    }
}
