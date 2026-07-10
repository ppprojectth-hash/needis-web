namespace Needis.Web.Helpers;

public static class UrlSafetyHelper
{
    // Allows relative paths (/Product, /Contact) and absolute http(s) links.
    // Rejects javascript:, data:, vbscript: and any other non-http(s) scheme.
    public static bool IsSafeLinkUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        var trimmed = url.Trim();

        if (trimmed.StartsWith('/') && !trimmed.StartsWith("//"))
            return true;

        if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
            return uri.Scheme is "http" or "https";

        return false;
    }
}
