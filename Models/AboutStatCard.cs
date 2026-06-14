using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class AboutStatCard
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string StatKey { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LabelTH { get; set; }

    [MaxLength(200)]
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

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
