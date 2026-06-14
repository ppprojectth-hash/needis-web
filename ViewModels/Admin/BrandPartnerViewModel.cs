using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class BrandPartnerViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string BrandName { get; set; } = string.Empty;

    public string? ExistingLogoUrl { get; set; }

    public IFormFile? LogoFile { get; set; }

    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    [MaxLength(100)]
    public string? BrandType { get; set; }

    public bool IsGlobalBrand { get; set; }

    public bool ShowOnPartnerSection { get; set; } = true;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
