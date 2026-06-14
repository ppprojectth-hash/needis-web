namespace Needis.Web.ViewModels.Admin;

public class MediaEditViewModel
{
    public int    Id               { get; set; }
    public string FileName         { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileUrl          { get; set; } = string.Empty;
    public string FileType         { get; set; } = string.Empty;
    public string ContentType      { get; set; } = string.Empty;
    public string FileExtension    { get; set; } = string.Empty;
    public long   FileSize         { get; set; }
    public string? UsageType      { get; set; }
    public string? RelatedModule  { get; set; }
    public int?   RelatedEntityId { get; set; }

    public string? TitleTH       { get; set; }
    public string? TitleEN       { get; set; }
    public string? AltTextTH     { get; set; }
    public string? AltTextEN     { get; set; }
    public string? CaptionTH     { get; set; }
    public string? CaptionEN     { get; set; }
    public string? DescriptionTH { get; set; }
    public string? DescriptionEN { get; set; }
    public bool    IsPublic      { get; set; }
    public bool    IsActive      { get; set; }
}
