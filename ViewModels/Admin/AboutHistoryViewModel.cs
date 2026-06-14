using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class AboutHistoryViewModel
{
    public int Id { get; set; }

    [Required, Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    public int Year { get; set; }

    [MaxLength(200)]
    public string? TitleTH { get; set; }

    [Required, MaxLength(200)]
    public string? TitleEN { get; set; }

    public string? DescriptionTH { get; set; }

    public string? DescriptionEN { get; set; }

    [MaxLength(50)]
    public string? Position { get; set; } = "left";

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
