using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class MediaFile
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Folder { get; set; }

    [Required, MaxLength(50)]
    public string FileType { get; set; } = "Other";

    [Required, MaxLength(150)]
    public string ContentType { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string FileExtension { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [MaxLength(200)]
    public string? TitleTH { get; set; }

    [MaxLength(200)]
    public string? TitleEN { get; set; }

    [MaxLength(300)]
    public string? AltTextTH { get; set; }

    [MaxLength(300)]
    public string? AltTextEN { get; set; }

    [MaxLength(500)]
    public string? CaptionTH { get; set; }

    [MaxLength(500)]
    public string? CaptionEN { get; set; }

    public string? DescriptionTH { get; set; }

    public string? DescriptionEN { get; set; }

    [MaxLength(100)]
    public string? UsageType { get; set; }

    [MaxLength(100)]
    public string? RelatedModule { get; set; }

    public int? RelatedEntityId { get; set; }

    public bool IsPublic { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(150)]
    public string? CreatedBy { get; set; }

    [MaxLength(150)]
    public string? UpdatedBy { get; set; }
}
