using Needis.Web.Models;

namespace Needis.Web.Services.Email;

public interface IEmailTemplateService
{
    string BuildQuotationAdminNotificationEmail(
        QuotationRequest request,
        List<QuotationRequestItem> items,
        string adminUrl);

    string BuildQuotationCustomerAutoReplyEmail(
        QuotationRequest request,
        List<QuotationRequestItem> items,
        string currentLanguage);

    string BuildContactMessageNotificationEmail(ContactMessage message, string? adminDetailUrl = null);
}
