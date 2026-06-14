using Needis.Web.Models;

namespace Needis.Web.ViewModels.Admin;

public class ProductSpecificationIndexViewModel
{
    public int    ProductId     { get; set; }
    public string? ProductNameEN { get; set; }
    public string? ProductNameTH { get; set; }
    public string? ProductSlug   { get; set; }
    public string? Keyword       { get; set; }
    public List<ProductSpecification> Specifications { get; set; } = new();
}
