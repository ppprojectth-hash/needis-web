using Needis.Web.ViewModels.Manual;

namespace Needis.Web.Services.Manual;

public interface IManualService
{
    Task<string> GetManualMarkdownAsync(string manualKey);
    Task<string> GetManualHtmlAsync(string manualKey);
    bool ManualExists(string manualKey);
    List<ManualMenuItemViewModel> GetAdminManualMenu();
    List<ManualMenuItemViewModel> GetCustomerManualMenu();
}
