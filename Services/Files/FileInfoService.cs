using Microsoft.AspNetCore.Hosting;
using Needis.Web.Services.Features;

namespace Needis.Web.Services.Files;

public class FileInfoService : IFileInfoService
{
    private readonly IWebHostEnvironment _env;
    private readonly IFeatureFlagService _features;

    public FileInfoService(IWebHostEnvironment env, IFeatureFlagService features)
    {
        _env      = env;
        _features = features;
    }

    public long? GetFileSizeBytes(string? fileUrl)
    {
        var path = ResolveLocalPath(fileUrl);
        if (path is null) return null;
        var fi = new FileInfo(path);
        return fi.Exists ? fi.Length : null;
    }

    public string GetFileSizeDisplay(string? fileUrl)
    {
        if (!_features.UploadedFileSizeEnabled) return string.Empty;
        if (string.IsNullOrWhiteSpace(fileUrl)) return "-";

        if (fileUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            fileUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return "External URL";

        var path = ResolveLocalPath(fileUrl);
        if (path is null) return "-";

        var fi = new FileInfo(path);
        if (!fi.Exists) return "File not found";

        return FormatBytes(fi.Length);
    }

    public bool FileExists(string? fileUrl)
    {
        var path = ResolveLocalPath(fileUrl);
        return path is not null && File.Exists(path);
    }

    // ── Private ──────────────────────────────────────────────────────────

    private string? ResolveLocalPath(string? fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return null;
        if (fileUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            fileUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return null;

        var relative = fileUrl.TrimStart('/');
        return Path.Combine(_env.WebRootPath, relative);
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024)             return $"{bytes} B";
        if (bytes < 1024 * 1024)     return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }
}
