namespace Needis.Web.Services.HomePopup;

public interface IHomePopupService
{
    Task<Models.HomePopup?> GetActivePopupAsync();
}
