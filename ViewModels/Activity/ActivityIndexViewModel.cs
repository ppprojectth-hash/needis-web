using Needis.Web.Models;

namespace Needis.Web.ViewModels.Activity;

public class ActivityIndexViewModel : SiteTextViewModelBase
{
    public string CurrentLanguage { get; set; } = "en";
    public ActivityPage? ActivityPage { get; set; }
    public List<ActivityTag> Tags { get; set; } = new();
    public List<Models.Activity> Activities { get; set; } = new();
    public List<Models.Activity> FeaturedActivities { get; set; } = new();
    public string? SelectedTagKey { get; set; }
    public ActivityTag? SelectedTag { get; set; }
    public string? Search { get; set; }
}
