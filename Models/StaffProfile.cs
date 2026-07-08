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

    [MaxLength(250)]
    public string? Slug { get; set; }

    [MaxLength(100)]
    public string? MobilePhone { get; set; }

    [MaxLength(250)]
    public string? Email { get; set; }

    [MaxLength(4000)]
    public string? BiographyTH { get; set; }

    [MaxLength(4000)]
    public string? BiographyEN { get; set; }

    [MaxLength(4000)]
    public string? AchievementTH { get; set; }

    [MaxLength(4000)]
    public string? AchievementEN { get; set; }

    [MaxLength(500)]
    public string? PdfFileUrl { get; set; }

    [MaxLength(250)]
    public string? PdfFileName { get; set; }

    public bool ShowContactInfo { get; set; } = true;

    public bool ShowDetailPage { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;

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
