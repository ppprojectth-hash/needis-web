using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class StaffProfileViewModel
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? EmployeeCode { get; set; }

    [MaxLength(200)]
    public string? FullNameTH { get; set; }

    [Required, MaxLength(200)]
    public string? FullNameEN { get; set; }

    [MaxLength(200)]
    public string? PositionTH { get; set; }

    [MaxLength(200)]
    public string? PositionEN { get; set; }

    [MaxLength(200)]
    public string? Department { get; set; }

    [MaxLength(100)]
    public string? StaffType { get; set; }

    public bool IsExpert { get; set; }

    public bool ShowPublic { get; set; }

    public string? ExistingImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}
