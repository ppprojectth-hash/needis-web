using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ServiceScopeItemViewModel
{
    public int Id { get; set; }

    [Required]
    public int ServiceDetailSectionId { get; set; }

    [MaxLength(200)]
    public string? ItemTitleTH { get; set; }

    [Required, MaxLength(200)]
    public string? ItemTitleEN { get; set; }

    public string? ItemDescriptionTH { get; set; }

    public string? ItemDescriptionEN { get; set; }

    [MaxLength(500)]
    public string? IconUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
