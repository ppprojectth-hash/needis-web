using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class HomeBanner
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string TitleTH { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string TitleEN { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? SubtitleTH { get; set; }

    [MaxLength(500)]
    public string? SubtitleEN { get; set; }

    [MaxLength(100)]
    public string? ButtonTextTH { get; set; }

    [MaxLength(100)]
    public string? ButtonTextEN { get; set; }

    [MaxLength(500)]
    public string? ButtonUrl { get; set; }

    [MaxLength(500)]
    public string? ImagePath { get; set; }

    // ── Media type extension ────────────────────────────────────────
    /// <summary>Image | Video | YouTube</summary>
    [MaxLength(50)]
    public string MediaType { get; set; } = "Image";

    /// <summary>Video URL (YouTube link or direct .mp4/.webm URL)</summary>
    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    /// <summary>Uploaded video file relative path</summary>
    [MaxLength(500)]
    public string? VideoFileUrl { get; set; }

    /// <summary>Optional smaller image for mobile screens</summary>
    [MaxLength(500)]
    public string? MobileImageUrl { get; set; }

    /// <summary>Optional smaller video for mobile screens</summary>
    [MaxLength(500)]
    public string? MobileVideoUrl { get; set; }

    public bool IsAutoplay           { get; set; } = true;
    public bool IsMuted              { get; set; } = true;
    public bool IsLoop               { get; set; } = true;
    public bool ShowControls         { get; set; } = false;
    public int  SlideDurationSeconds { get; set; } = 6;

    [MaxLength(50)]
    public string OverlayStyle { get; set; } = "dark"; // dark | light | gradient | none

    [MaxLength(50)]
    public string TextPosition { get; set; } = "left"; // left | center | right

    // ── Schedule / order ────────────────────────────────────────────
    public DateTime? StartDate    { get; set; }
    public DateTime? EndDate      { get; set; }
    public int       DisplayOrder { get; set; }
    public bool      IsActive     { get; set; } = true;
    public DateTime  CreatedAt    { get; set; }
    public DateTime  UpdatedAt    { get; set; }
}
