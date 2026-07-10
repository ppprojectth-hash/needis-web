using System.Text.RegularExpressions;

namespace Needis.Web.Services.Media;

// Supports: youtube.com/watch?v=, youtu.be/, youtube.com/embed/, youtube.com/shorts/, youtube.com/live/
public class YoutubeUrlService : IYoutubeUrlService
{
    private static readonly Regex VideoIdRegex = new(
        @"(?:youtu\.be/|youtube\.com/(?:watch\?(?:.*&)?v=|embed/|shorts/|live/))([A-Za-z0-9_\-]{11})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string? ExtractVideoId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var m = VideoIdRegex.Match(url.Trim());
        return m.Success ? m.Groups[1].Value : null;
    }

    public bool IsValidYoutubeUrl(string? url) => ExtractVideoId(url) is not null;

    public string? ToEmbedUrl(string? url)
    {
        var id = ExtractVideoId(url);
        return id is null ? null : $"https://www.youtube.com/embed/{id}";
    }
}
