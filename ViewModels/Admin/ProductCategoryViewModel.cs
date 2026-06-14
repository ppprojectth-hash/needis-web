using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ProductCategoryViewModel
{
    public int ProductCategoryId { get; set; }

    [Display(Name = "Category Name (Thai)")]
    [MaxLength(200)]
    public string? CategoryNameTH { get; set; }

    [Required(ErrorMessage = "Category Name (English) is required.")]
    [Display(Name = "Category Name (English)")]
    [MaxLength(200)]
    public string? CategoryNameEN { get; set; }

    [Required(ErrorMessage = "Slug is required.")]
    [Display(Name = "Slug")]
    [MaxLength(200)]
    public string? Slug { get; set; }

    [Display(Name = "Short Description (Thai)")]
    [MaxLength(500)]
    public string? ShortDescriptionTH { get; set; }

    [Display(Name = "Short Description (English)")]
    [MaxLength(500)]
    public string? ShortDescriptionEN { get; set; }

    public string? ImageUrl { get; set; }

    [Display(Name = "Category Image")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Display Order")]
    [Range(0, 9999)]
    public int DisplayOrder { get; set; } = 0;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

/// <summary>Used by the Index list view to carry per-row data.</summary>
public class CategoryListItem
{
    public int    Id           { get; set; }
    public string NameEN       { get; set; } = string.Empty;
    public string NameTH       { get; set; } = string.Empty;
    public string Slug         { get; set; } = string.Empty;
    public string? ImagePath   { get; set; }
    public int    DisplayOrder { get; set; }
    public bool   IsActive     { get; set; }
    public int    ProductCount { get; set; }
}
