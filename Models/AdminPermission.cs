using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class AdminPermission
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string PermissionKey { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string PermissionGroup { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string PermissionName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsActive     { get; set; } = true;
    public bool IsDelete     { get; set; } = false;

    public DateTime  CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)] public string? CreatedBy { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }

    public ICollection<AdminRolePermission> RolePermissions { get; set; } = new List<AdminRolePermission>();
}
