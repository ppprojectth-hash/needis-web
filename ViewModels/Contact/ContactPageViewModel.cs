using Needis.Web.Models;

namespace Needis.Web.ViewModels.Contact;

public class ContactPageViewModel : SiteTextViewModelBase
{
    public string         CurrentLanguage { get; init; } = "en";
    public SiteSetting?   SiteSetting     { get; init; }
    public FooterContact? FooterContact   { get; init; }
    public ContactFormViewModel Form      { get; init; } = new();
}
