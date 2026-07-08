using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class SiteSettingViewModel
{
    public int SiteSettingId { get; set; }

    [Display(Name = "Company Name (Thai)")]
    [MaxLength(200)]
    public string? CompanyNameTH { get; set; }

    [Required(ErrorMessage = "Company Name (English) is required.")]
    [Display(Name = "Company Name (English)")]
    [MaxLength(200)]
    public string? CompanyNameEN { get; set; }

    public string? LogoUrl { get; set; }

    public string? FaviconUrl { get; set; }

    [Display(Name = "Main Color")]
    [MaxLength(20)]
    [RegularExpression(@"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$",
        ErrorMessage = "Main Color must be a valid hex code, e.g. #2d4199 or #RGB.")]
    public string? MainColor { get; set; } = "#2d4199";

    [Display(Name = "Phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Display(Name = "Address (Thai)")]
    [MaxLength(500)]
    public string? AddressTH { get; set; }

    [Display(Name = "Address (English)")]
    [MaxLength(500)]
    public string? AddressEN { get; set; }

    [Display(Name = "Facebook URL")]
    [MaxLength(500)]
    public string? FacebookUrl { get; set; }

    [Display(Name = "Line URL")]
    [MaxLength(500)]
    public string? LineUrl { get; set; }

    [Display(Name = "LinkedIn URL")]
    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }

    [Display(Name = "Show Map on About Page")]
    public bool ShowMapOnAboutPage { get; set; } = true;

    [Display(Name = "Google Map URL")]
    [MaxLength(1000)]
    public string? GoogleMapUrl { get; set; }

    [Display(Name = "Google Map Embed URL")]
    [MaxLength(2000)]
    public string? GoogleMapEmbedUrl { get; set; }

    [Display(Name = "Map Title (TH)")]
    [MaxLength(200)]
    public string? MapTitleTH { get; set; }

    [Display(Name = "Map Title (EN)")]
    [MaxLength(200)]
    public string? MapTitleEN { get; set; }

    [Display(Name = "Map Description (TH)")]
    [MaxLength(500)]
    public string? MapDescriptionTH { get; set; }

    [Display(Name = "Map Description (EN)")]
    [MaxLength(500)]
    public string? MapDescriptionEN { get; set; }

    public bool IsActive { get; set; } = true;

    [Display(Name = "Logo Image")]
    public IFormFile? LogoFile { get; set; }

    [Display(Name = "Favicon Image")]
    public IFormFile? FaviconFile { get; set; }
}
