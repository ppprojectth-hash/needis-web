using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ProductSale
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public DateTime SaleDate { get; set; }

    public int Quantity { get; set; }

    [MaxLength(200)]
    public string? CustomerName { get; set; }

    [MaxLength(100)]
    public string? ReferenceNo { get; set; }

    public bool CountInAboutStats { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public Product? Product { get; set; }
}
