using Needis.Web.Models;

namespace Needis.Web.Services.Quotation;

public interface IQuotationCartService
{
    Task<QuotationCart> GetOrCreateCartAsync(HttpContext httpContext);
    Task<int> GetCartItemCountAsync(HttpContext httpContext);
    Task AddProductAsync(HttpContext httpContext, int productId, int quantity, string? itemNote);
    Task AddServiceAsync(HttpContext httpContext, int serviceId, int quantity, string? itemNote);
    Task UpdateQuantityAsync(HttpContext httpContext, int itemId, int quantity);
    Task UpdateNoteAsync(HttpContext httpContext, int itemId, string? itemNote);
    Task RemoveItemAsync(HttpContext httpContext, int itemId);
    Task ClearCartAsync(HttpContext httpContext);
    Task<QuotationCart?> GetActiveCartWithItemsAsync(HttpContext httpContext);
}
