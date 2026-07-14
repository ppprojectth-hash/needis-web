using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string NameTH { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string NameEN { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Slug { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    [MaxLength(200)]
    public string? Brand { get; set; }

    [MaxLength(200)]
    public string? Model { get; set; }

    [MaxLength(100)]
    public string? Sku { get; set; }

    [MaxLength(500)]
    public string? ShortDescriptionTH { get; set; }

    [MaxLength(500)]
    public string? ShortDescriptionEN { get; set; }

    public string? FullDescriptionTH { get; set; }

    public string? FullDescriptionEN { get; set; }

    public string? SpecificationTH { get; set; }

    public string? SpecificationEN { get; set; }

    public decimal? Price { get; set; }

    public bool IsPriceVisible { get; set; } = true;

    [MaxLength(500)]
    public string? MainImagePath { get; set; }

    [MaxLength(500)]
    public string? BrochureFilePath { get; set; }

    public int DisplayOrder { get; set; }

    // Reused as the "Hot Product" marketing flag (see Admin UI label).
    public bool IsFeatured { get; set; }

    public bool IsPromotion { get; set; } = false;

    [MaxLength(200)]
    public string? PromotionLabelTH { get; set; }

    [MaxLength(200)]
    public string? PromotionLabelEN { get; set; }

    public bool IsActive { get; set; } = true;

    // ── YouTube Video ────────────────────────────────────────────────────────
    [MaxLength(500)]
    public string? YoutubeVideoUrl { get; set; }

    [MaxLength(200)]
    public string? YoutubeVideoTitleTH { get; set; }

    [MaxLength(200)]
    public string? YoutubeVideoTitleEN { get; set; }

    [MaxLength(1000)]
    public string? YoutubeVideoDescriptionTH { get; set; }

    [MaxLength(1000)]
    public string? YoutubeVideoDescriptionEN { get; set; }

    public bool ShowYoutubeVideo { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ProductCategory Category { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    public ICollection<ProductSale> ProductSales { get; set; } = new List<ProductSale>();

    public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
}
