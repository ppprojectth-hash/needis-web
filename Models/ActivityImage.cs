using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ActivityImage
{
    public int  Id                    { get; set; }
    public int  ActivityId            { get; set; }
    public int? ActivityDetailBlockId { get; set; }

    [MaxLength(500)] public string? ImageUrl    { get; set; }
    [MaxLength(200)] public string? ImageTitleTH { get; set; }
    [MaxLength(200)] public string? ImageTitleEN { get; set; }
    [MaxLength(500)] public string? CaptionTH   { get; set; }
    [MaxLength(500)] public string? CaptionEN   { get; set; }
    [MaxLength(200)] public string? AltTextTH   { get; set; }
    [MaxLength(200)] public string? AltTextEN   { get; set; }
    [MaxLength(100)] public string? ImageType   { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsCover      { get; set; } = false;
    public bool IsActive     { get; set; } = true;
    public bool IsDelete     { get; set; } = false;

    public DateTime  CreatedAt  { get; set; }
    public DateTime? UpdatedAt  { get; set; }
    [MaxLength(100)] public string? CreatedBy { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }

    // Navigation
    public Activity?             Activity            { get; set; }
    public ActivityDetailBlock?  ActivityDetailBlock { get; set; }
}
