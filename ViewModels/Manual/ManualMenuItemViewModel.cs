namespace Needis.Web.ViewModels.Manual;

public class ManualMenuItemViewModel
{
    public string Key     { get; init; } = "";
    public string TitleTH { get; init; } = "";
    public string TitleEN { get; init; } = "";
    public string Url     { get; init; } = "";
    public string IconCss { get; init; } = "bi-file-text";
    public bool   IsActive { get; init; }
}
