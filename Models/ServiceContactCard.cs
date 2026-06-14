using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ServiceContactCard
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    [MaxLength(200)]
    public string? TitleTH { get; set; }

    [MaxLength(200)]
    public string? TitleEN { get; set; }

    [MaxLength(500)]
    public string? DescriptionTH { get; set; }

    [MaxLength(500)]
    public string? DescriptionEN { get; set; }

    [MaxLength(200)]
    public string? PrimaryButtonTextTH { get; set; }

    [MaxLength(200)]
    public string? PrimaryButtonTextEN { get; set; }

    [MaxLength(500)]
    public string? PrimaryButtonUrl { get; set; }

    [MaxLength(200)]
    public string? SecondaryButtonTextTH { get; set; }

    [MaxLength(200)]
    public string? SecondaryButtonTextEN { get; set; }

    [MaxLength(500)]
    public string? SecondaryButtonUrl { get; set; }

    [MaxLength(200)]
    public string? ContactLabelTH { get; set; }

    [MaxLength(200)]
    public string? ContactLabelEN { get; set; }

    [MaxLength(200)]
    public string? ContactValue { get; set; }

    [MaxLength(500)]
    public string? ContactIconUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public Service? Service { get; set; }
}
