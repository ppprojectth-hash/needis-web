using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class StaffProfile
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? EmployeeCode { get; set; }

    [MaxLength(200)]
    public string? FullNameTH { get; set; }

    [MaxLength(200)]
    public string? FullNameEN { get; set; }

    [MaxLength(200)]
    public string? PositionTH { get; set; }

    [MaxLength(200)]
    public string? PositionEN { get; set; }

    [MaxLength(200)]
    public string? Department { get; set; }

    [MaxLength(100)]
    public string? StaffType { get; set; }

    public bool IsExpert { get; set; } = false;

    public bool ShowPublic { get; set; } = false;

    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
