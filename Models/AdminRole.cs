using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class AdminRole
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string RoleKey { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string RoleName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsSystem { get; set; } = false;
    public bool IsActive  { get; set; } = true;
    public bool IsDelete  { get; set; } = false;

    public DateTime  CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt  { get; set; }

    [MaxLength(100)] public string? CreatedBy { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }

    public ICollection<AdminRolePermission> RolePermissions { get; set; } = new List<AdminRolePermission>();
    public ICollection<AdminUser>           AdminUsers      { get; set; } = new List<AdminUser>();
}
