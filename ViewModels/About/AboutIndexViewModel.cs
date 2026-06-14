using Needis.Web.Models;

namespace Needis.Web.ViewModels.About;

public class AboutIndexViewModel
{
    public string CurrentLanguage { get; set; } = "en";
    public List<AboutSection> Sections { get; set; } = new();
    public List<AboutCompanyHistory> Histories { get; set; } = new();
    public List<AboutStatCardDisplayViewModel> StatCards { get; set; } = new();
    public List<BrandPartner> BrandPartners { get; set; } = new();
    public List<StaffProfile> PublicStaffProfiles { get; set; } = new();
}
