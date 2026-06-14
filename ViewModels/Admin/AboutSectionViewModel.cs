using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class AboutSectionViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string SectionKey { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? TitleTH { get; set; }

    [Required, MaxLength(200)]
    public string? TitleEN { get; set; }

    [MaxLength(300)]
    public string? SubtitleTH { get; set; }

    [MaxLength(300)]
    public string? SubtitleEN { get; set; }

    public string? DescriptionTH { get; set; }

    public string? DescriptionEN { get; set; }

    public string? ExistingImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    [MaxLength(100)]
    public string? LayoutType { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
