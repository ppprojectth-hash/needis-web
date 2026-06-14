using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Quotation;

public class QuotationCartSubmitViewModel
{
    [Required, MaxLength(200)]
    public string? CustomerName { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [Required, EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? PreferredContactMethod { get; set; }

    [Required, MaxLength(500)]
    public string? Subject { get; set; }

    public string? Message { get; set; }
}
