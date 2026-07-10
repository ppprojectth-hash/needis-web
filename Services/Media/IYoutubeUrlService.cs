namespace Needis.Web.Services.Media;

public interface IYoutubeUrlService
{
    /// <summary>Extracts the 11-character video ID from any supported YouTube URL format.</summary>
    string? ExtractVideoId(string? url);

    /// <summary>Converts any supported YouTube URL into a https://www.youtube.com/embed/{id} URL.</summary>
    string? ToEmbedUrl(string? url);

    bool IsValidYoutubeUrl(string? url);
}
