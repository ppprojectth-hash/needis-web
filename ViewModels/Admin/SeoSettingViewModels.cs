using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class SeoSettingListItemViewModel
{
    public int     Id               { get; set; }
    public string  PageKey          { get; set; } = string.Empty;
    public string? EntityType       { get; set; }
    public int?    EntityId         { get; set; }
    public string? RoutePath        { get; set; }
    public string? MetaTitleEN      { get; set; }
    public string? MetaTitleTH      { get; set; }
    public bool    IncludeInSitemap { get; set; }
    public bool    IsActive         { get; set; }
    public DateTime? UpdatedAt      { get; set; }
}

public class SeoSettingEditViewModel
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

    [Range(0.0, 1.0)]
    public decimal Priority { get; set; } = 0.8m;

    [MaxLength(50)]
    public string ChangeFrequency { get; set; } = "weekly";

    public bool IncludeInSitemap { get; set; } = true;
    public bool IsActive         { get; set; } = true;
}

public class SeoRedirectListItemViewModel
{
    public int    Id         { get; set; }
    public string SourcePath { get; set; } = string.Empty;
    public string TargetPath { get; set; } = string.Empty;
    public int    StatusCode { get; set; }
    public bool   IsActive   { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SeoRedirectEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(500)]
    public string SourcePath { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TargetPath { get; set; } = string.Empty;

    [Range(301, 302)]
    public int StatusCode { get; set; } = 301;

    public bool IsActive { get; set; } = true;
}
