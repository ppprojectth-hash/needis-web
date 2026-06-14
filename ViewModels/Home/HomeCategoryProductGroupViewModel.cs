namespace Needis.Web.ViewModels.Home;

public class HomeCategoryProductGroupViewModel
{
    public int     CategoryId          { get; init; }
    public string  CategoryNameTH      { get; init; } = "";
    public string  CategoryNameEN      { get; init; } = "";
    public string? CategoryDescriptionTH { get; init; }
    public string? CategoryDescriptionEN { get; init; }
    public string? CategorySlug        { get; init; }
    public string? CategoryImagePath   { get; init; }
    public List<Models.Product> Products { get; init; } = [];
}
