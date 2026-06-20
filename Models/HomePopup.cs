using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class HomePopup
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string TitleTH { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string TitleEN { get; set; } = string.Empty;

    public string? DescriptionTH { get; set; }
    public string? DescriptionEN { get; set; }

    [Required, MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? MobileImageUrl { get; set; }

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    [MaxLength(100)]
    public string? ButtonTextTH { get; set; }

    [MaxLength(100)]
    public string? ButtonTextEN { get; set; }

    [MaxLength(100)]
    public string? PopupType { get; set; }

    public int  DisplayOrder          { get; set; } = 0;
    public bool IsActive              { get; set; } = true;
    public bool IsDelete              { get; set; } = false;
    public bool ShowOnlyHomePage      { get; set; } = true;
    public bool ShowOncePerSession    { get; set; } = false;
    public bool ShowOncePerDay        { get; set; } = true;
    public bool OpenLinkInNewTab      { get; set; } = true;
    public int  DisplayDelaySeconds   { get; set; } = 1;

    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc   { get; set; }

    public DateTime  CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(150)]
    public string? CreatedBy { get; set; }

    [MaxLength(150)]
    public string? UpdatedBy { get; set; }
}
