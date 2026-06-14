using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class AboutSection
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string SectionKey { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? TitleTH { get; set; }

    [MaxLength(200)]
    public string? TitleEN { get; set; }

    [MaxLength(300)]
    public string? SubtitleTH { get; set; }

    [MaxLength(300)]
    public string? SubtitleEN { get; set; }

    public string? DescriptionTH { get; set; }

    public string? DescriptionEN { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

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

    public ICollection<AboutSectionItem> Items { get; set; } = new List<AboutSectionItem>();
}
