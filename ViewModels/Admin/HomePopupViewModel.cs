using System.ComponentModel.DataAnnotations;
using Needis.Web.Helpers;

namespace Needis.Web.ViewModels.Admin;

public class HomePopupViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Title TH")]
    public string? TitleTH { get; set; }

    [Required, Display(Name = "Title EN")]
    public string? TitleEN { get; set; }

    [Display(Name = "Description TH")]
    public string? DescriptionTH { get; set; }

    [Display(Name = "Description EN")]
    public string? DescriptionEN { get; set; }

    public string? ImageUrl { get; set; }

    [Display(Name = "Popup Image")]
    public IFormFile? ImageFile { get; set; }

    public string? MobileImageUrl { get; set; }

    [Display(Name = "Mobile Image (optional)")]
    public IFormFile? MobileImageFile { get; set; }

    [Display(Name = "Link URL")]
    public string? LinkUrl { get; set; }

    [Display(Name = "Button Text TH")]
    public string? ButtonTextTH { get; set; }

    [Display(Name = "Button Text EN")]
    public string? ButtonTextEN { get; set; }

    [Display(Name = "Popup Type")]
    public string? PopupType { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; } = 0;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Show only on Home Page")]
    public bool ShowOnlyHomePage { get; set; } = true;

    [Display(Name = "Show once per session")]
    public bool ShowOncePerSession { get; set; } = false;

    [Display(Name = "Show once per day")]
    public bool ShowOncePerDay { get; set; } = true;

    [Display(Name = "Open link in new tab")]
    public bool OpenLinkInNewTab { get; set; } = true;

    [Display(Name = "Display Delay (seconds)")]
    [Range(0, 10, ErrorMessage = "Delay must be between 0 and 10 seconds.")]
    public int DisplayDelaySeconds { get; set; } = 1;

    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
            yield return new ValidationResult("Start Date must be on or before End Date.", [nameof(StartDate), nameof(EndDate)]);

        // LinkUrl is intentionally optional — some popups are informational only —
        // but if one is provided it must be a safe, navigable URL.
        if (!string.IsNullOrWhiteSpace(LinkUrl) && !UrlSafetyHelper.IsSafeLinkUrl(LinkUrl))
        {
            yield return new ValidationResult(
                "Link URL must start with http://, https://, or / (javascript:, data:, and similar links are not allowed).",
                [nameof(LinkUrl)]);
        }
    }
}
