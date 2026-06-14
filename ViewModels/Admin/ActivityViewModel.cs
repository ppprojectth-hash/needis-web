using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ActivityViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ActivitySlug { get; set; } = string.Empty;

    [MaxLength(200)] public string? ActivityTitleTH { get; set; }
    [Required, MaxLength(200)] public string? ActivityTitleEN { get; set; }

    [MaxLength(500)] public string? ShortDescriptionTH { get; set; }
    [MaxLength(500)] public string? ShortDescriptionEN { get; set; }

    public string? SummaryTH        { get; set; }
    public string? SummaryEN        { get; set; }
    public string? ContentPreviewTH { get; set; }
    public string? ContentPreviewEN { get; set; }

    public string?    ExistingCoverImageUrl  { get; set; }
    public IFormFile? CoverImageFile         { get; set; }
    public string?    ExistingBannerImageUrl { get; set; }
    public IFormFile? BannerImageFile        { get; set; }

    public DateTime? ActivityDate  { get; set; }
    public DateTime? PublishedDate { get; set; }

    [MaxLength(200)] public string? LocationTH { get; set; }
    [MaxLength(200)] public string? LocationEN { get; set; }
    [MaxLength(200)] public string? AuthorName { get; set; }

    public bool IsFeatured  { get; set; }
    public bool IsPublished { get; set; }
    public int  DisplayOrder { get; set; }

    [MaxLength(200)] public string? MetaTitleTH       { get; set; }
    [MaxLength(200)] public string? MetaTitleEN       { get; set; }
    [MaxLength(500)] public string? MetaDescriptionTH { get; set; }
    [MaxLength(500)] public string? MetaDescriptionEN { get; set; }

    public bool IsActive { get; set; } = true;
}
