using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Required, MaxLength(500)]
    public string ImagePath { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;
}
