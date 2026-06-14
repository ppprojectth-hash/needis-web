using Needis.Web.Models;

namespace Needis.Web.ViewModels.Activity;

public class ActivityDetailViewModel
{
    public string CurrentLanguage { get; set; } = "en";
    public Models.Activity Activity { get; set; } = null!;
    public List<ActivityTag> Tags { get; set; } = new();
    public List<ActivityDetailBlock> DetailBlocks { get; set; } = new();
    public List<ActivityImage> Images { get; set; } = new();
    public List<ActivityRelatedItem> RelatedItems { get; set; } = new();
    public List<Models.Activity> MoreActivities { get; set; } = new();
}
