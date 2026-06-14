using System.ComponentModel.DataAnnotations;
using ProductModel = Needis.Web.Models.Product;
using ServiceModel = Needis.Web.Models.Service;

namespace Needis.Web.ViewModels.Quotation;

public class QuotationRequestFormViewModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    [MaxLength(200)]
    public string? CustomerName { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? PreferredContactMethod { get; set; }

    [Required(ErrorMessage = "Please enter a subject.")]
    [MaxLength(500)]
    public string? Subject { get; set; }

    public string? Message { get; set; }

    // ── Product ─────────────────────────────────────────────────────────────
    public int? ProductId { get; set; }

    public string? ProductSlug { get; set; }

    // ── Service ─────────────────────────────────────────────────────────────
    public int? ServiceId { get; set; }

    public string? ServiceSlug { get; set; }

    // ── Common item fields ───────────────────────────────────────────────────
    [Range(1, 9999, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; } = 1;

    [MaxLength(500)]
    public string? ItemNote { get; set; }

    public string RequestType { get; set; } = "General";

    public string CurrentLanguage { get; set; } = "en";

    // Loaded from DB — never trusted from form POST
    public ProductModel? Product { get; set; }

    public ServiceModel? Service { get; set; }
}
