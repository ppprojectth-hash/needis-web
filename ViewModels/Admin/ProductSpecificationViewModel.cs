using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ProductSpecificationViewModel
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    public string? ProductNameEN { get; set; }
    public string? ProductNameTH { get; set; }

    [MaxLength(150)]
    public string? SpecGroupTH { get; set; }

    [MaxLength(150)]
    public string? SpecGroupEN { get; set; }

    [MaxLength(200)]
    public string? SpecNameTH { get; set; }

    [Required, MaxLength(200)]
    public string? SpecNameEN { get; set; }

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

    public int  DisplayOrder { get; set; }
    public bool IsHighlight  { get; set; }
    public bool IsActive     { get; set; } = true;
}
