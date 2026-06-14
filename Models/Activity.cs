using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class Activity
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string ActivitySlug { get; set; } = string.Empty;

    [MaxLength(200)] public string? ActivityTitleTH    { get; set; }
    [MaxLength(200)] public string? ActivityTitleEN    { get; set; }
    [MaxLength(500)] public string? ShortDescriptionTH { get; set; }
    [MaxLength(500)] public string? ShortDescriptionEN { get; set; }

    public string? SummaryTH        { get; set; }
    public string? SummaryEN        { get; set; }
    public string? ContentPreviewTH { get; set; }
    public string? ContentPreviewEN { get; set; }

    [MaxLength(500)] public string? CoverImageUrl  { get; set; }
    [MaxLength(500)] public string? BannerImageUrl { get; set; }

    public DateTime? ActivityDate    { get; set; }
    public DateTime? PublishedDate   { get; set; }

    [MaxLength(200)] public string? LocationTH  { get; set; }
    [MaxLength(200)] public string? LocationEN  { get; set; }
    [MaxLength(200)] public string? AuthorName  { get; set; }

    public bool IsFeatured   { get; set; } = false;
    public bool IsPublished  { get; set; } = false;
    public int  DisplayOrder { get; set; }
    public int  ViewCount    { get; set; } = 0;

    [MaxLength(200)] public string? MetaTitleTH       { get; set; }
    [MaxLength(200)] public string? MetaTitleEN       { get; set; }
    [MaxLength(500)] public string? MetaDescriptionTH { get; set; }
    [MaxLength(500)] public string? MetaDescriptionEN { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDelete { get; set; } = false;

    public DateTime  CreatedAt  { get; set; }
    public DateTime? UpdatedAt  { get; set; }
    [MaxLength(100)] public string? CreatedBy { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }

    // Navigation
    public ICollection<ActivityTagMap>      ActivityTagMaps { get; set; } = new List<ActivityTagMap>();
    public ICollection<ActivityDetailBlock> DetailBlocks    { get; set; } = new List<ActivityDetailBlock>();
    public ICollection<ActivityImage>       Images          { get; set; } = new List<ActivityImage>();
    public ICollection<ActivityRelatedItem> RelatedItems    { get; set; } = new List<ActivityRelatedItem>();
}
