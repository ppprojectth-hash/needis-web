using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class StaffProfileViewModel
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? EmployeeCode { get; set; }

    [Required, MaxLength(200)]
    public string? FullNameEN { get; set; }

    [MaxLength(200)]
    public string? FullNameTH { get; set; }

    [MaxLength(200)]
    public string? PositionEN { get; set; }

    [MaxLength(200)]
    public string? PositionTH { get; set; }

    [MaxLength(250)]
    [Display(Name = "Slug")]
    public string? Slug { get; set; }

    [MaxLength(200)]
    public string? Department { get; set; }

    [MaxLength(100)]
    public string? StaffType { get; set; }

    public bool IsExpert { get; set; }

    public bool ShowPublic { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // ── Contact ──────────────────────────────────────────────────
    [MaxLength(100)]
    [Display(Name = "Mobile Phone")]
    public string? MobilePhone { get; set; }

    [MaxLength(250)]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string? Email { get; set; }

    [Display(Name = "Show Contact Info publicly")]
    public bool ShowContactInfo { get; set; } = true;

    // ── Profile Detail ────────────────────────────────────────────
    [MaxLength(4000)]
    [Display(Name = "Biography (TH)")]
    public string? BiographyTH { get; set; }

    [MaxLength(4000)]
    [Display(Name = "Biography (EN)")]
    public string? BiographyEN { get; set; }

    [MaxLength(4000)]
    [Display(Name = "Achievements / Experience (TH)")]
    public string? AchievementTH { get; set; }

    [MaxLength(4000)]
    [Display(Name = "Achievements / Experience (EN)")]
    public string? AchievementEN { get; set; }

    [Display(Name = "Enable Public Detail Page")]
    public bool ShowDetailPage { get; set; } = true;

    // ── Files ─────────────────────────────────────────────────────
    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Profile Photo")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingPdfUrl { get; set; }

    public string? ExistingPdfName { get; set; }

    [Display(Name = "Profile PDF")]
    public IFormFile? PdfFile { get; set; }

    // ── Dates ─────────────────────────────────────────────────────
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
