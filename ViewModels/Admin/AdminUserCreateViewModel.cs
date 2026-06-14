using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class AdminUserCreateViewModel
{
    [Required, MaxLength(100)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "Display Name")]
    public string? DisplayName { get; set; }

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; } = "Admin";

    [Display(Name = "Admin Role")]
    public int? AdminRoleId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
