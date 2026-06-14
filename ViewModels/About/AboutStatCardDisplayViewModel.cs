namespace Needis.Web.ViewModels.About;

public class AboutStatCardDisplayViewModel
{
    public int Id { get; set; }
    public string StatKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public string? IconUrl { get; set; }
    public string? CardStyle { get; set; }
    public int DisplayOrder { get; set; }
    public int CalculatedValue { get; set; }

    // Ready-to-display string: Prefix + CalculatedValue + Suffix
    public string DisplayValue { get; set; } = string.Empty;
}
