namespace Needis.Web.ViewModels.Admin;

public class QuotationRequestListItemViewModel
{
    public int Id { get; set; }
    public string RequestNo { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Status { get; set; } = "New";
    public string RequestType { get; set; } = "General";
    public int ItemCount { get; set; }
    public string? ItemSummary { get; set; }
    public DateTime CreatedAt { get; set; }
}
