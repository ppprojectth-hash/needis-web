using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;

namespace Needis.Web.Services.Media;

public class MediaFileService : IMediaFileService
{
    private static readonly string[] AllowedImageExtensions =
        [".jpg", ".jpeg", ".png", ".webp", ".svg", ".gif", ".ico"];

    private static readonly string[] AllowedDocExtensions =
        [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx"];

    private static readonly string[] BlockedExtensions =
        [".exe", ".bat", ".cmd", ".sh", ".js", ".html", ".php", ".aspx", ".dll", ".msi", ".ps1"];

    private const long MaxImageBytes = 5 * 1024 * 1024;   // 5 MB
    private const long MaxDocBytes   = 20 * 1024 * 1024;  // 20 MB

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<MediaFileService> _logger;

    public MediaFileService(AppDbContext db, IWebHostEnvironment env, ILogger<MediaFileService> logger)
    {
        _db     = db;
        _env    = env;
        _logger = logger;
    }

    // ── Upload ───────────────────────────────────────────────────────────────

    public async Task<MediaFile> UploadAsync(
        IFormFile file,
        string usageType,
        string? relatedModule = null,
        int? relatedEntityId = null,
        string? folder = null,
        string? uploadedBy = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsAllowedFile(file, out var error))
            throw new InvalidOperationException(error);

        var ext      = Path.GetExtension(file.FileName).ToLowerInvariant();
        var now      = DateTime.UtcNow;
        var subPath  = folder ?? $"media/{now:yyyy/MM}";

        // Build physical directory — only use safe path segments
        var safeSub  = string.Join("/", subPath.Split('/', '\\')
            .Select(s => Regex.Replace(s, @"[^\w\-]", "_")));

        var dir = Path.Combine(_env.WebRootPath, "uploads", safeSub);
        Directory.CreateDirectory(dir);

        var guid       = Guid.NewGuid().ToString("N");
        var fileName   = $"{guid}{ext}";
        var physPath   = Path.Combine(dir, fileName);
        var fileUrl    = $"/uploads/{safeSub}/{fileName}";
        var fileType   = GetFileType(ext, file.ContentType);
        var origName   = SanitizeFileName(Path.GetFileName(file.FileName));

        await using (var stream = new FileStream(physPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var media = new MediaFile
        {
            FileName         = fileName,
            OriginalFileName = origName,
            FileUrl          = fileUrl,
            Folder           = subPath,
            FileType         = fileType,
            ContentType      = file.ContentType?.Length > 150
                ? file.ContentType[..150]
                : (file.ContentType ?? "application/octet-stream"),
            FileExtension    = ext,
            FileSize         = file.Length,
            UsageType        = usageType,
            RelatedModule    = relatedModule,
            RelatedEntityId  = relatedEntityId,
            IsPublic         = true,
            IsActive         = true,
            IsDelete         = false,
            CreatedAt        = now,
            CreatedBy        = uploadedBy,
        };

        _db.MediaFiles.Add(media);
        await _db.SaveChangesAsync(cancellationToken);
        return media;
    }

    // ── SoftDelete ───────────────────────────────────────────────────────────

    public async Task<bool> SoftDeleteAsync(int id, string? updatedBy, CancellationToken cancellationToken = default)
    {
        var media = await _db.MediaFiles.FirstOrDefaultAsync(m => m.Id == id && !m.IsDelete, cancellationToken);
        if (media is null) return false;

        media.IsDelete  = true;
        media.IsActive  = false;
        media.UpdatedAt = DateTime.UtcNow;
        media.UpdatedBy = updatedBy;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    // ── UpdateMetadata ───────────────────────────────────────────────────────

    public async Task<bool> UpdateMetadataAsync(
        int id,
        string? titleTH, string? titleEN,
        string? altTextTH, string? altTextEN,
        string? captionTH, string? captionEN,
        string? descriptionTH, string? descriptionEN,
        bool isPublic, bool isActive,
        string? updatedBy,
        CancellationToken cancellationToken = default)
    {
        var media = await _db.MediaFiles.FirstOrDefaultAsync(m => m.Id == id && !m.IsDelete, cancellationToken);
        if (media is null) return false;

        media.TitleTH       = titleTH;
        media.TitleEN       = titleEN;
        media.AltTextTH     = altTextTH;
        media.AltTextEN     = altTextEN;
        media.CaptionTH     = captionTH;
        media.CaptionEN     = captionEN;
        media.DescriptionTH = descriptionTH;
        media.DescriptionEN = descriptionEN;
        media.IsPublic      = isPublic;
        media.IsActive      = isActive;
        media.UpdatedAt     = DateTime.UtcNow;
        media.UpdatedBy     = updatedBy;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    // ── IsAllowedFile ────────────────────────────────────────────────────────

    public bool IsAllowedFile(IFormFile file, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (file is null || file.Length == 0)
        {
            errorMessage = "No file provided.";
            return false;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (BlockedExtensions.Contains(ext))
        {
            errorMessage = $"File type '{ext}' is not allowed for security reasons.";
            return false;
        }

        var isImage = AllowedImageExtensions.Contains(ext);
        var isDoc   = AllowedDocExtensions.Contains(ext);

        if (!isImage && !isDoc)
        {
            errorMessage = $"File type '{ext}' is not supported. Allowed: jpg, jpeg, png, webp, svg, gif, ico, pdf, doc, docx, xls, xlsx, ppt, pptx.";
            return false;
        }

        var maxBytes = isImage ? MaxImageBytes : MaxDocBytes;
        if (file.Length > maxBytes)
        {
            var maxMb = maxBytes / (1024 * 1024);
            errorMessage = $"File size exceeds the {maxMb} MB limit.";
            return false;
        }

        return true;
    }

    // ── GetFileType ──────────────────────────────────────────────────────────

    public string GetFileType(string extension, string contentType)
    {
        if (AllowedImageExtensions.Contains(extension.ToLowerInvariant()))
            return "Image";

        return extension.ToLowerInvariant() switch
        {
            ".pdf"                   => "Pdf",
            ".doc" or ".docx"        => "Document",
            ".xls" or ".xlsx"        => "Document",
            ".ppt" or ".pptx"        => "Document",
            _                        => "Other",
        };
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string SanitizeFileName(string name)
    {
        // Remove path traversal chars and limit length
        var safe = Regex.Replace(name, @"[\\/:*?""<>|]", "_");
        return safe.Length > 255 ? safe[..255] : safe;
    }
}
