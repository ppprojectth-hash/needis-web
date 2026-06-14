using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ProductViewModel
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public int ProductCategoryId { get; set; }

    [Display(Name = "Product Name (Thai)")]
    [MaxLength(300)]
    public string? ProductNameTH { get; set; }

    [Required(ErrorMessage = "Product Name (English) is required.")]
    [Display(Name = "Product Name (English)")]
    [MaxLength(300)]
    public string? ProductNameEN { get; set; }

    [Required(ErrorMessage = "Slug is required.")]
    [Display(Name = "Slug")]
    [MaxLength(300)]
    public string? Slug { get; set; }

    [Display(Name = "Brand")]
    [MaxLength(200)]
    public string? Brand { get; set; }

    [Display(Name = "Model")]
    [MaxLength(200)]
    public string? Model { get; set; }

    [Display(Name = "Part Number / SKU")]
    [MaxLength(100)]
    public string? PartNumber { get; set; }

    [Display(Name = "Short Description (Thai)")]
    [MaxLength(500)]
    public string? ShortDescriptionTH { get; set; }

    [Display(Name = "Short Description (English)")]
    [MaxLength(500)]
    public string? ShortDescriptionEN { get; set; }

    [Display(Name = "Description (Thai)")]
    public string? DescriptionTH { get; set; }

    [Display(Name = "Description (English)")]
    public string? DescriptionEN { get; set; }

    [Display(Name = "Specification (Thai)")]
    public string? SpecificationTH { get; set; }

    [Display(Name = "Specification (English)")]
    public string? SpecificationEN { get; set; }

    [Display(Name = "Price")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or greater.")]
    public decimal? Price { get; set; }

    [Display(Name = "Show Price")]
    public bool IsPriceVisible { get; set; } = true;

    public string? MainImageUrl { get; set; }

    [Display(Name = "Product Image")]
    public IFormFile? MainImageFile { get; set; }

    public string? BrochureFileUrl { get; set; }

    [Display(Name = "Brochure (PDF)")]
    public IFormFile? BrochureFile { get; set; }

    [Display(Name = "Display Order")]
    [Range(0, 9999)]
    public int DisplayOrder { get; set; } = 0;

    [Display(Name = "Featured")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class ProductListItem
{
    public int    Id            { get; set; }
    public string NameEN        { get; set; } = string.Empty;
    public string NameTH        { get; set; } = string.Empty;
    public string CategoryNameEN { get; set; } = string.Empty;
    public string? Brand        { get; set; }
    public string? Model        { get; set; }
    public string? Sku          { get; set; }
    public string? MainImagePath { get; set; }
    public decimal? Price       { get; set; }
    public bool   IsPriceVisible { get; set; }
    public bool   IsFeatured    { get; set; }
    public bool   IsActive      { get; set; }
    public int    DisplayOrder  { get; set; }
}
