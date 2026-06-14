using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ActivityImageViewModel
{
    public int Id { get; set; }

    [Required]
    public int  ActivityId            { get; set; }
    public int? ActivityDetailBlockId { get; set; }

    public string?    ExistingImageUrl { get; set; }
    public IFormFile? ImageFile        { get; set; }

    [MaxLength(200)] public string? ImageTitleTH { get; set; }
    [MaxLength(200)] public string? ImageTitleEN { get; set; }
    [MaxLength(500)] public string? CaptionTH   { get; set; }
    [MaxLength(500)] public string? CaptionEN   { get; set; }
    [MaxLength(200)] public string? AltTextTH   { get; set; }
    [MaxLength(200)] public string? AltTextEN   { get; set; }
    [MaxLength(100)] public string? ImageType   { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsCover      { get; set; }
    public bool IsActive     { get; set; } = true;
}
