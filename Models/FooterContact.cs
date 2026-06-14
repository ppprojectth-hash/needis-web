using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class FooterContact
{
    public int Id { get; set; }

    [MaxLength(1000)]
    public string? DescriptionTH { get; set; }

    [MaxLength(1000)]
    public string? DescriptionEN { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(500)]
    public string? AddressTH { get; set; }

    [MaxLength(500)]
    public string? AddressEN { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? FacebookUrl { get; set; }

    [MaxLength(500)]
    public string? LineUrl { get; set; }

    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime UpdatedAt { get; set; }
}
