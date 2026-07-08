using Needis.Web.Models;

namespace Needis.Web.ViewModels.Product;

public class ProductDetailViewModel : SiteTextViewModelBase
{
    public Models.Product                Product         { get; init; } = null!;
    public List<Models.Product>          RelatedProducts { get; init; } = [];
    public List<ProductSpecification>    Specifications  { get; init; } = [];
    public string                        CurrentLanguage { get; init; } = "en";
    public string?                       YoutubeEmbedUrl { get; init; }
}
