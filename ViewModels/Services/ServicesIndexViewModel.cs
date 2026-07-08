using Needis.Web.Models;

namespace Needis.Web.ViewModels.Services;

public class ServicesIndexViewModel : SiteTextViewModelBase
{
    public string CurrentLanguage { get; set; } = "en";
    public ServicePage? ServicePage { get; set; }
    public List<Service> Services { get; set; } = new();
    public List<Service> FeaturedServices { get; set; } = new();
}
