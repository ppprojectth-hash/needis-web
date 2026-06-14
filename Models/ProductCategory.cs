using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ProductCategory
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string NameTH { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameEN { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ShortDescriptionTH { get; set; }

    [MaxLength(500)]
    public string? ShortDescriptionEN { get; set; }

    [MaxLength(500)]
    public string? ImagePath { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
