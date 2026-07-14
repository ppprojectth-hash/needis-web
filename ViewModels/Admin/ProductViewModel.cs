using System.ComponentModel.DataAnnotations;
using Needis.Web.Helpers;

namespace Needis.Web.ViewModels.Admin;

public class ProductViewModel : IValidatableObject
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

    [Display(Name = "Hot Product")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Promotion")]
    public bool IsPromotion { get; set; }

    [Display(Name = "Promotion Label (Thai)")]
    [MaxLength(200)]
    public string? PromotionLabelTH { get; set; }

    [Display(Name = "Promotion Label (English)")]
    [MaxLength(200)]
    public string? PromotionLabelEN { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    // ── YouTube Video ────────────────────────────────────────────────────────

    [Display(Name = "Show YouTube Video")]
    public bool ShowYoutubeVideo { get; set; } = false;

    [Display(Name = "YouTube Video URL")]
    [MaxLength(500)]
    public string? YoutubeVideoUrl { get; set; }

    [Display(Name = "Video Title (TH)")]
    [MaxLength(200)]
    public string? YoutubeVideoTitleTH { get; set; }

    [Display(Name = "Video Title (EN)")]
    [MaxLength(200)]
    public string? YoutubeVideoTitleEN { get; set; }

    [Display(Name = "Video Description (TH)")]
    [MaxLength(1000)]
    public string? YoutubeVideoDescriptionTH { get; set; }

    [Display(Name = "Video Description (EN)")]
    [MaxLength(1000)]
    public string? YoutubeVideoDescriptionEN { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(YoutubeVideoUrl) && !YoutubeHelper.IsValidYoutubeUrl(YoutubeVideoUrl))
            yield return new ValidationResult(
                "Please enter a valid YouTube URL. Supported: youtube.com/watch, youtu.be, embed, or shorts.",
                [nameof(YoutubeVideoUrl)]);
    }
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
    public bool   IsPromotion   { get; set; }
    public bool   IsActive      { get; set; }
    public int    DisplayOrder  { get; set; }
    public bool   HasVideo      { get; set; }
}
