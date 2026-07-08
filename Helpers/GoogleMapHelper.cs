namespace Needis.Web.Helpers;

public static class GoogleMapHelper
{
    private static readonly HashSet<string> SafeLinkHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "maps.app.goo.gl",
        "google.com",
        "www.google.com",
        "maps.google.com",
    };

    public static bool IsSafeGoogleMapUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)) return false;
        return uri.Scheme is "https" or "http"
               && SafeLinkHosts.Contains(uri.Host);
    }

    public static bool IsSafeGoogleMapEmbedUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)) return false;
        return uri.Scheme == "https"
               && (uri.Host == "www.google.com" || uri.Host == "google.com")
               && uri.AbsolutePath.StartsWith("/maps/embed", StringComparison.OrdinalIgnoreCase);
    }
}
