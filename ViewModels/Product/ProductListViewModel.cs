using Needis.Web.Models;

namespace Needis.Web.ViewModels.Product;

public class ProductListViewModel
{
    public List<Models.Product>         Products              { get; init; } = [];
    public List<ProductCategory>        Categories            { get; init; } = [];
    public string?                      SelectedCategorySlug  { get; init; }
    public int?                         SelectedCategoryId    { get; init; }
    public ProductCategory?             SelectedCategory      { get; init; }
    public string?                      Search                { get; init; }
    public string                       CurrentLanguage       { get; init; } = "en";
}
