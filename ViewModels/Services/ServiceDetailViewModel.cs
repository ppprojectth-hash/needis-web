using Needis.Web.Models;

namespace Needis.Web.ViewModels.Services;

public class ServiceDetailViewModel
{
    public string CurrentLanguage { get; set; } = "en";
    public Service Service { get; set; } = null!;
    public List<ServiceDetailSection> DetailSections { get; set; } = new();
    public List<ServiceContactCard> ContactCards { get; set; } = new();
    public List<Service> RelatedServices { get; set; } = new();
}
