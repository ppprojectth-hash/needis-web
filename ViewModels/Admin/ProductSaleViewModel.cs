using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ProductSaleViewModel
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public DateTime SaleDate { get; set; } = DateTime.Today;

    [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; } = 1;

    [MaxLength(200)]
    public string? CustomerName { get; set; }

    [MaxLength(100)]
    public string? ReferenceNo { get; set; }

    public bool CountInAboutStats { get; set; } = true;

    public bool IsActive { get; set; } = true;
}
