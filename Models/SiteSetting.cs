using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class SiteSetting
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string CompanyNameTH { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string CompanyNameEN { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoPath { get; set; }

    [MaxLength(500)]
    public string? FaviconPath { get; set; }

    [MaxLength(20)]
    public string MainColor { get; set; } = "#2d4199";

    [MaxLength(200)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    [MaxLength(500)]
    public string? AddressTH { get; set; }

    [MaxLength(500)]
    public string? AddressEN { get; set; }

    [MaxLength(500)]
    public string? FacebookUrl { get; set; }

    [MaxLength(500)]
    public string? LineUrl { get; set; }

    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }

    [Required, MaxLength(5)]
    public string DefaultLanguage { get; set; } = "EN";

    public bool IsActive { get; set; } = true;

    public DateTime UpdatedAt { get; set; }
}
