using Needis.Web.Models;

namespace Needis.Web.Services;

public interface ISiteSettingService
{
    Task<SiteSetting?> GetActiveAsync();
}
