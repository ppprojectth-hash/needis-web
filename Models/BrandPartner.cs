using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class BrandPartner
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string BrandName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    [MaxLength(100)]
    public string? BrandType { get; set; }

    public bool IsGlobalBrand { get; set; } = false;

    public bool ShowOnPartnerSection { get; set; } = true;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
