using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ServiceViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ServiceCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string ServiceSlug { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ServiceNameTH { get; set; }

    [Required, MaxLength(200)]
    public string? ServiceNameEN { get; set; }

    [MaxLength(500)]
    public string? ShortDescriptionTH { get; set; }

    [MaxLength(500)]
    public string? ShortDescriptionEN { get; set; }

    public string? FullDescriptionTH { get; set; }

    public string? FullDescriptionEN { get; set; }

    public string? ExistingCoverImageUrl { get; set; }
    public string? ExistingBannerImageUrl { get; set; }
    public string? ExistingIconUrl { get; set; }

    public IFormFile? CoverImageFile { get; set; }
    public IFormFile? BannerImageFile { get; set; }
    public IFormFile? IconFile { get; set; }

    [MaxLength(200)]
    public string? DetailTitleTH { get; set; }

    [MaxLength(200)]
    public string? DetailTitleEN { get; set; }

    [MaxLength(300)]
    public string? DetailSubtitleTH { get; set; }

    [MaxLength(300)]
    public string? DetailSubtitleEN { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; } = true;
}
