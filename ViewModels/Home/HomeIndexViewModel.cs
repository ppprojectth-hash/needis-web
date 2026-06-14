using Needis.Web.Models;

namespace Needis.Web.ViewModels.Home;

public class HomeIndexViewModel
{
    public SiteSetting?          SiteSetting      { get; init; }
    public List<HomeBanner>      Banners          { get; init; } = [];
    public List<ProductCategory> Categories       { get; init; } = [];
    public List<Models.Product>  FeaturedProducts { get; init; } = [];
    public List<HomeCategoryProductGroupViewModel> ProductGroups { get; init; } = [];
    public List<Models.Service>  FeaturedServices { get; init; } = [];
    public List<Models.Activity> LatestActivities { get; init; } = [];
    public string                CurrentLanguage  { get; init; } = "en";
}
