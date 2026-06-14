using Needis.Web.Models;

namespace Needis.Web.ViewModels.Quotation;

public class QuotationCartViewModel
{
    public string CurrentLanguage { get; set; } = "en";
    public List<QuotationCartItem> Items { get; set; } = new();
    public int TotalItems => Items.Count;

    // Customer contact form
    public string? CustomerName            { get; set; }
    public string? CompanyName             { get; set; }
    public string? Email                   { get; set; }
    public string? Phone                   { get; set; }
    public string? PreferredContactMethod  { get; set; }
    public string? Subject                 { get; set; }
    public string? Message                 { get; set; }
}
