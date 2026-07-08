using Needis.Web.Models;

namespace Needis.Web.ViewModels.About;

public class AboutIndexViewModel : SiteTextViewModelBase
{
    public string CurrentLanguage { get; set; } = "en";
    public List<AboutSection> Sections { get; set; } = new();
    public List<AboutCompanyHistory> Histories { get; set; } = new();
    public List<AboutStatCardDisplayViewModel> StatCards { get; set; } = new();
    public List<BrandPartner> BrandPartners { get; set; } = new();
    public List<StaffProfile> PublicStaffProfiles { get; set; } = new();

    // Google Map / Location
    public bool ShowMapOnAboutPage { get; set; } = false;
    public string? GoogleMapUrl { get; set; }
    public string? GoogleMapEmbedUrl { get; set; }
    public string? MapTitleTH { get; set; }
    public string? MapTitleEN { get; set; }
    public string? MapDescriptionTH { get; set; }
    public string? MapDescriptionEN { get; set; }

    // Company contact info (from SiteSetting)
    public string? AddressTH { get; set; }
    public string? AddressEN { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
}
