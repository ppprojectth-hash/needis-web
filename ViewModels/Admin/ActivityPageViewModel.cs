using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ActivityPageViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string PageKey { get; set; } = string.Empty;

    [MaxLength(200)] public string? TitleTH { get; set; }
    [Required, MaxLength(200)] public string? TitleEN { get; set; }
    [MaxLength(300)] public string? SubtitleTH { get; set; }
    [MaxLength(300)] public string? SubtitleEN { get; set; }

    public string? DescriptionTH { get; set; }
    public string? DescriptionEN { get; set; }

    public string?    ExistingBgImageUrl  { get; set; }
    public IFormFile? BackgroundImageFile { get; set; }

    [MaxLength(200)] public string? BreadcrumbTextTH { get; set; }
    [MaxLength(200)] public string? BreadcrumbTextEN { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsActive     { get; set; } = true;
}
