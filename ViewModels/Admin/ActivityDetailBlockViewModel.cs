using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ActivityDetailBlockViewModel
{
    public int Id { get; set; }

    [Required]
    public int ActivityId { get; set; }

    [Required, MaxLength(100)]
    public string BlockType { get; set; } = string.Empty;

    [MaxLength(200)] public string? BlockTitleTH    { get; set; }
    [MaxLength(200)] public string? BlockTitleEN    { get; set; }
    [MaxLength(300)] public string? BlockSubtitleTH { get; set; }
    [MaxLength(300)] public string? BlockSubtitleEN { get; set; }

    public string? BlockContentTH { get; set; }
    public string? BlockContentEN { get; set; }

    public string?    ExistingImageUrl { get; set; }
    public IFormFile? BlockImageFile   { get; set; }

    [MaxLength(500)] public string? VideoUrl { get; set; }

    [MaxLength(200)] public string? ButtonTextTH { get; set; }
    [MaxLength(200)] public string? ButtonTextEN { get; set; }
    [MaxLength(500)] public string? ButtonUrl    { get; set; }
    [MaxLength(100)] public string? LayoutType   { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsActive     { get; set; } = true;
}
