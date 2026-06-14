using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ServiceDetailSectionViewModel
{
    public int Id { get; set; }

    [Required]
    public int ServiceId { get; set; }

    [Required, MaxLength(100)]
    public string SectionKey { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SectionTitleTH { get; set; }

    [Required, MaxLength(200)]
    public string? SectionTitleEN { get; set; }

    [MaxLength(300)]
    public string? SectionSubtitleTH { get; set; }

    [MaxLength(300)]
    public string? SectionSubtitleEN { get; set; }

    public string? SectionDescriptionTH { get; set; }

    public string? SectionDescriptionEN { get; set; }

    public string? ExistingImageUrl { get; set; }

    public IFormFile? SectionImageFile { get; set; }

    [MaxLength(100)]
    public string? LayoutType { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
