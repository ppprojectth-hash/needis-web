using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class QuotationRequest
{
    public int Id { get; set; }

    [Required, MaxLength(30)]
    public string RequestNo { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? CustomerName { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? PreferredContactMethod { get; set; }

    [MaxLength(500)]
    public string? Subject { get; set; }

    public string? Message { get; set; }

    [Required, MaxLength(50)]
    public string Status { get; set; } = "New";

    [MaxLength(50)]
    public string RequestType { get; set; } = "General";

    [MaxLength(10)]
    public string? Language { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public string? AdminRemark { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<QuotationRequestItem> Items { get; set; } = new List<QuotationRequestItem>();
}
