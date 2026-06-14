using Needis.Web.Models;

namespace Needis.Web.ViewModels.Admin;

public class MediaLibraryIndexViewModel
{
    public string? Keyword       { get; set; }
    public string? FileType      { get; set; }
    public string? UsageType     { get; set; }
    public string? RelatedModule { get; set; }

    public List<MediaFile> MediaFiles    { get; set; } = new();
    public int TotalCount                { get; set; }
    public int ImageCount                { get; set; }
    public int DocumentCount             { get; set; }
    public int PdfCount                  { get; set; }
    public long TotalStorageBytes        { get; set; }

    public string FormatStorage()
    {
        if (TotalStorageBytes < 1024)            return $"{TotalStorageBytes} B";
        if (TotalStorageBytes < 1024 * 1024)     return $"{TotalStorageBytes / 1024.0:F1} KB";
        if (TotalStorageBytes < 1024L * 1024 * 1024) return $"{TotalStorageBytes / 1024.0 / 1024.0:F1} MB";
        return $"{TotalStorageBytes / 1024.0 / 1024.0 / 1024.0:F2} GB";
    }
}
