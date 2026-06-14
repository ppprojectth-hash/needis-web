using Needis.Web.Models;

namespace Needis.Web.ViewModels.Admin;

public class QuotationRequestDetailViewModel
{
    public QuotationRequest Request { get; set; } = null!;
    public List<QuotationRequestItem> Items { get; set; } = new();
    public string? NewStatus { get; set; }
    public string? AdminRemark { get; set; }
}
