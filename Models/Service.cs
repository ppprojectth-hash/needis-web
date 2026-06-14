using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class Service
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ServiceCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string ServiceSlug { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ServiceNameTH { get; set; }

    [MaxLength(200)]
    public string? ServiceNameEN { get; set; }

    [MaxLength(500)]
    public string? ShortDescriptionTH { get; set; }

    [MaxLength(500)]
    public string? ShortDescriptionEN { get; set; }

    public string? FullDescriptionTH { get; set; }

    public string? FullDescriptionEN { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    [MaxLength(500)]
    public string? BannerImageUrl { get; set; }

    [MaxLength(500)]
    public string? IconUrl { get; set; }

    [MaxLength(200)]
    public string? DetailTitleTH { get; set; }

    [MaxLength(200)]
    public string? DetailTitleEN { get; set; }

    [MaxLength(300)]
    public string? DetailSubtitleTH { get; set; }

    [MaxLength(300)]
    public string? DetailSubtitleEN { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsFeatured { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public ICollection<ServiceDetailSection> DetailSections { get; set; } = new List<ServiceDetailSection>();
    public ICollection<ServiceContactCard>   ContactCards   { get; set; } = new List<ServiceContactCard>();
}
