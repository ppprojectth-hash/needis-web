using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ActivityDetailBlock
{
    public int Id         { get; set; }
    public int ActivityId { get; set; }

    [Required, MaxLength(100)]
    public string BlockType { get; set; } = string.Empty;

    [MaxLength(200)] public string? BlockTitleTH    { get; set; }
    [MaxLength(200)] public string? BlockTitleEN    { get; set; }
    [MaxLength(300)] public string? BlockSubtitleTH { get; set; }
    [MaxLength(300)] public string? BlockSubtitleEN { get; set; }

    public string? BlockContentTH { get; set; }
    public string? BlockContentEN { get; set; }

    [MaxLength(500)] public string? ImageUrl  { get; set; }
    [MaxLength(500)] public string? VideoUrl  { get; set; }

    [MaxLength(200)] public string? ButtonTextTH { get; set; }
    [MaxLength(200)] public string? ButtonTextEN { get; set; }
    [MaxLength(500)] public string? ButtonUrl    { get; set; }
    [MaxLength(100)] public string? LayoutType   { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsActive     { get; set; } = true;
    public bool IsDelete     { get; set; } = false;

    public DateTime  CreatedAt  { get; set; }
    public DateTime? UpdatedAt  { get; set; }
    [MaxLength(100)] public string? CreatedBy { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }

    // Navigation
    public Activity?                 Activity { get; set; }
    public ICollection<ActivityImage> Images  { get; set; } = new List<ActivityImage>();
}
