using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class AboutStatCardViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string StatKey { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LabelTH { get; set; }

    [Required, MaxLength(200)]
    public string? LabelEN { get; set; }

    [MaxLength(500)]
    public string? DescriptionTH { get; set; }

    [MaxLength(500)]
    public string? DescriptionEN { get; set; }

    [Required, MaxLength(100)]
    public string SourceType { get; set; } = "Manual";

    public int? ManualValue { get; set; }

    [MaxLength(50)]
    public string? Prefix { get; set; }

    [MaxLength(50)]
    public string? Suffix { get; set; }

    [MaxLength(500)]
    public string? IconUrl { get; set; }

    [MaxLength(100)]
    public string? CardStyle { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
