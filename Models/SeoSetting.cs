using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class SeoSetting
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string PageKey { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    [MaxLength(500)]
    public string? RoutePath { get; set; }

    [MaxLength(300)]
    public string? MetaTitleTH { get; set; }

    [MaxLength(300)]
    public string? MetaTitleEN { get; set; }

    [MaxLength(1000)]
    public string? MetaDescriptionTH { get; set; }

    [MaxLength(1000)]
    public string? MetaDescriptionEN { get; set; }

    [MaxLength(500)]
    public string? MetaKeywordsTH { get; set; }

    [MaxLength(500)]
    public string? MetaKeywordsEN { get; set; }

    [MaxLength(300)]
    public string? OgTitleTH { get; set; }

    [MaxLength(300)]
    public string? OgTitleEN { get; set; }

    [MaxLength(1000)]
    public string? OgDescriptionTH { get; set; }

    [MaxLength(1000)]
    public string? OgDescriptionEN { get; set; }

    [MaxLength(500)]
    public string? OgImageUrl { get; set; }

    [MaxLength(500)]
    public string? CanonicalUrl { get; set; }

    [MaxLength(100)]
    public string? Robots { get; set; } = "index, follow";

    public decimal Priority { get; set; } = 0.8m;

    [MaxLength(50)]
    public string ChangeFrequency { get; set; } = "weekly";

    public bool IncludeInSitemap { get; set; } = true;
    public bool IsActive  { get; set; } = true;
    public bool IsDelete  { get; set; } = false;

    public DateTime  CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt  { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
