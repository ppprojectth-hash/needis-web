using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class MediaUploadViewModel
{
    [Required(ErrorMessage = "Please select a file.")]
    public IFormFile? File { get; set; }

    public string? UsageType      { get; set; }
    public string? RelatedModule  { get; set; }
    public int?    RelatedEntityId { get; set; }
    public string? Folder         { get; set; }

    public string? TitleTH       { get; set; }
    public string? TitleEN       { get; set; }
    public string? AltTextTH     { get; set; }
    public string? AltTextEN     { get; set; }
    public string? CaptionTH     { get; set; }
    public string? CaptionEN     { get; set; }
    public string? DescriptionTH { get; set; }
    public string? DescriptionEN { get; set; }
    public bool    IsPublic      { get; set; } = true;
    public bool    IsActive      { get; set; } = true;
}
