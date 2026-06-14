using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class AboutCompanyHistory
{
    public int Id { get; set; }

    public int Year { get; set; }

    [MaxLength(200)]
    public string? TitleTH { get; set; }

    [MaxLength(200)]
    public string? TitleEN { get; set; }

    public string? DescriptionTH { get; set; }

    public string? DescriptionEN { get; set; }

    [MaxLength(50)]
    public string? Position { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
