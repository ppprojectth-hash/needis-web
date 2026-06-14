using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class HomeBannerViewModel : IValidatableObject
{
    public int HomeBannerId { get; set; }

    // ── Content ─────────────────────────────────────────────────────
    [Display(Name = "Title (Thai)")]
    [MaxLength(300)]
    public string? TitleTH { get; set; }

    [Required(ErrorMessage = "Title (English) is required.")]
    [Display(Name = "Title (English)")]
    [MaxLength(300)]
    public string? TitleEN { get; set; }

    [Display(Name = "Subtitle (Thai)")]
    [MaxLength(500)]
    public string? SubtitleTH { get; set; }

    [Display(Name = "Subtitle (English)")]
    [MaxLength(500)]
    public string? SubtitleEN { get; set; }

    [Display(Name = "Button Text (Thai)")]
    [MaxLength(100)]
    public string? ButtonTextTH { get; set; }

    [Display(Name = "Button Text (English)")]
    [MaxLength(100)]
    public string? ButtonTextEN { get; set; }

    [Display(Name = "Button URL")]
    [MaxLength(500)]
    public string? ButtonUrl { get; set; }

    // ── Media ────────────────────────────────────────────────────────
    /// <summary>Image | Video | YouTube</summary>
    [Required(ErrorMessage = "Media type is required.")]
    [Display(Name = "Media Type")]
    public string MediaType { get; set; } = "Image";

    // Image
    public string? ImageUrl { get; set; }

    [Display(Name = "Banner Image")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Mobile Image")]
    public string? MobileImageUrl { get; set; }

    [Display(Name = "Mobile Image File")]
    public IFormFile? MobileImageFile { get; set; }

    // Video
    [Display(Name = "Video URL")]
    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    public string? VideoFileUrl { get; set; }

    [Display(Name = "Video File (.mp4, .webm, .mov — max 50 MB)")]
    public IFormFile? VideoFile { get; set; }

    [Display(Name = "Mobile Video URL")]
    [MaxLength(500)]
    public string? MobileVideoUrl { get; set; }

    [Display(Name = "Mobile Video File")]
    public IFormFile? MobileVideoFile { get; set; }

    // ── Playback settings ────────────────────────────────────────────
    [Display(Name = "Autoplay")]
    public bool IsAutoplay { get; set; } = true;

    [Display(Name = "Muted")]
    public bool IsMuted { get; set; } = true;

    [Display(Name = "Loop")]
    public bool IsLoop { get; set; } = true;

    [Display(Name = "Show Controls")]
    public bool ShowControls { get; set; } = false;

    [Display(Name = "Slide Duration (seconds)")]
    [Range(3, 20, ErrorMessage = "Duration must be between 3 and 20 seconds.")]
    public int SlideDurationSeconds { get; set; } = 6;

    // ── Appearance ───────────────────────────────────────────────────
    [Display(Name = "Overlay Style")]
    public string OverlayStyle { get; set; } = "dark";

    [Display(Name = "Text Position")]
    public string TextPosition { get; set; } = "left";

    // ── Schedule / display ───────────────────────────────────────────
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    [Display(Name = "Display Order")]
    [Range(0, 9999)]
    public int DisplayOrder { get; set; } = 0;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    // ── Validation ───────────────────────────────────────────────────
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate.Value < StartDate.Value)
            yield return new ValidationResult("End Date must be >= Start Date.", [nameof(EndDate)]);

        // Image type: needs image file or existing image
        if (MediaType == "Image" &&
            string.IsNullOrEmpty(ImageUrl) &&
            (ImageFile == null || ImageFile.Length == 0))
        {
            yield return new ValidationResult(
                "A banner image is required for Image type.",
                [nameof(ImageFile)]);
        }

        // YouTube type: needs video URL
        if (MediaType == "YouTube" && string.IsNullOrEmpty(VideoUrl))
        {
            yield return new ValidationResult(
                "YouTube URL is required.",
                [nameof(VideoUrl)]);
        }

        // Video type: needs file or URL
        if (MediaType == "Video" &&
            string.IsNullOrEmpty(VideoFileUrl) &&
            string.IsNullOrEmpty(VideoUrl) &&
            (VideoFile == null || VideoFile.Length == 0))
        {
            yield return new ValidationResult(
                "A video file or video URL is required for Video type.",
                [nameof(VideoFile)]);
        }
    }
}
