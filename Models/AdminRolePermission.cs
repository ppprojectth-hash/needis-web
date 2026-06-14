using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class AdminRolePermission
{
    public int Id                { get; set; }
    public int AdminRoleId       { get; set; }
    public int AdminPermissionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    public AdminRole?       AdminRole       { get; set; }
    public AdminPermission? AdminPermission { get; set; }
}
