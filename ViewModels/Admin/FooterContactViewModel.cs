using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class FooterContactViewModel
{
    public int FooterContactId { get; set; }

    [Display(Name = "Company Description (Thai)")]
    [MaxLength(1000)]
    public string? CompanyDescriptionTH { get; set; }

    [Display(Name = "Company Description (English)")]
    [MaxLength(1000)]
    public string? CompanyDescriptionEN { get; set; }

    [Display(Name = "Address (Thai)")]
    [MaxLength(500)]
    public string? AddressTH { get; set; }

    [Display(Name = "Address (English)")]
    [MaxLength(500)]
    public string? AddressEN { get; set; }

    [Display(Name = "Phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Display(Name = "Email")]
    [MaxLength(200)]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    [Display(Name = "Facebook URL")]
    [MaxLength(500)]
    [Url(ErrorMessage = "Facebook URL must be a valid URL.")]
    public string? FacebookUrl { get; set; }

    [Display(Name = "Line URL")]
    [MaxLength(500)]
    public string? LineUrl { get; set; }

    [Display(Name = "LinkedIn URL")]
    [MaxLength(500)]
    [Url(ErrorMessage = "LinkedIn URL must be a valid URL.")]
    public string? LinkedInUrl { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
