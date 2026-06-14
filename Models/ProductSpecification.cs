using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ProductSpecification
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    [MaxLength(150)]
    public string? SpecGroupTH { get; set; }

    [MaxLength(150)]
    public string? SpecGroupEN { get; set; }

    [MaxLength(200)]
    public string? SpecNameTH { get; set; }

    [Required, MaxLength(200)]
    public string SpecNameEN { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? SpecValueTH { get; set; }

    [MaxLength(500)]
    public string? SpecValueEN { get; set; }

    [MaxLength(100)]
    public string? UnitTH { get; set; }

    [MaxLength(100)]
    public string? UnitEN { get; set; }

    [MaxLength(500)]
    public string? RemarkTH { get; set; }

    [MaxLength(500)]
    public string? RemarkEN { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsHighlight { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(150)]
    public string? CreatedBy { get; set; }

    [MaxLength(150)]
    public string? UpdatedBy { get; set; }

    public Product? Product { get; set; }
}
