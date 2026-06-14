using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class AdminUserEditViewModel
{
    public int Id { get; set; }

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

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
