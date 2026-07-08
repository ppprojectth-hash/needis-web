using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class SiteTextViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Key { get; set; } = string.Empty;

    public bool IsNew { get; set; }

    [Required, MaxLength(100)]
    public string Page { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Section { get; set; }

    [Required, MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    public string? TextTH { get; set; }
    public string? TextEN { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public string TextType { get; set; } = "text";

    public int  DisplayOrder { get; set; }
    public bool IsActive     { get; set; } = true;
}
