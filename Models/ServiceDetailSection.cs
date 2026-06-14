using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ServiceDetailSection
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    [Required, MaxLength(100)]
    public string SectionKey { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SectionTitleTH { get; set; }

    [MaxLength(200)]
    public string? SectionTitleEN { get; set; }

    [MaxLength(300)]
    public string? SectionSubtitleTH { get; set; }

    [MaxLength(300)]
    public string? SectionSubtitleEN { get; set; }

    public string? SectionDescriptionTH { get; set; }

    public string? SectionDescriptionEN { get; set; }

    [MaxLength(500)]
    public string? SectionImageUrl { get; set; }

    [MaxLength(100)]
    public string? LayoutType { get; set; }

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

    public ICollection<ServiceScopeItem> ScopeItems { get; set; } = new List<ServiceScopeItem>();
}
