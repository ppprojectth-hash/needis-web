using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class AboutSectionItemViewModel
{
    public int Id { get; set; }

    public int AboutSectionId { get; set; }

    [MaxLength(200)]
    public string? TitleTH { get; set; }

    [MaxLength(200)]
    public string? TitleEN { get; set; }

    public string? DescriptionTH { get; set; }

    public string? DescriptionEN { get; set; }

    [MaxLength(500)]
    public string? IconUrl { get; set; }

    public string? ExistingImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
