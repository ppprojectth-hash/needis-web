using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class SiteText
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Key { get; set; } = string.Empty;

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

    [Required, MaxLength(20)]
    public string TextType { get; set; } = "text";

    public int  DisplayOrder { get; set; } = 0;
    public bool IsActive     { get; set; } = true;
    public bool IsDelete     { get; set; } = false;

    public DateTime  CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    [MaxLength(150)]
    public string? CreatedBy { get; set; }

    [MaxLength(150)]
    public string? UpdatedBy { get; set; }
}
