using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class QuotationCart
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string CartToken { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? Language { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public bool IsSubmitted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public ICollection<QuotationCartItem> Items { get; set; } = new List<QuotationCartItem>();
}
