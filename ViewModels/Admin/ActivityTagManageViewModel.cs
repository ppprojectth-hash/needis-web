namespace Needis.Web.ViewModels.Admin;

public class ActivityTagManageViewModel
{
    public int    ActivityId      { get; set; }
    public string ActivityTitleEN { get; set; } = string.Empty;
    public List<ActivityTagCheckItem> Tags { get; set; } = new();
    public int?   PrimaryTagId   { get; set; }
}

public class ActivityTagCheckItem
{
    public int     TagId      { get; set; }
    public string  TagNameEN  { get; set; } = string.Empty;
    public string? TagColor   { get; set; }
    public bool    IsSelected { get; set; }
    public bool    IsPrimary  { get; set; }
}
