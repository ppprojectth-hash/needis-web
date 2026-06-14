namespace Needis.Web.ViewModels.Manual;

public class ManualPageViewModel
{
    public string ManualKey   { get; init; } = "";
    public string Title       { get; init; } = "";
    public string HtmlContent { get; init; } = "";
    public bool   IsAdminPage { get; init; }
    public List<ManualMenuItemViewModel> MenuItems { get; init; } = [];
}
