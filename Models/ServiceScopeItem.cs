using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ServiceScopeItem
{
    public int Id { get; set; }

    public int ServiceDetailSectionId { get; set; }

    [MaxLength(200)]
    public string? ItemTitleTH { get; set; }

    [MaxLength(200)]
    public string? ItemTitleEN { get; set; }

    public string? ItemDescriptionTH { get; set; }

    public string? ItemDescriptionEN { get; set; }

    [MaxLength(500)]
    public string? IconUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public ServiceDetailSection? ServiceDetailSection { get; set; }
}
