using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class AdminRoleListItemViewModel
{
    public int     Id              { get; set; }
    public string  RoleKey         { get; set; } = string.Empty;
    public string  RoleName        { get; set; } = string.Empty;
    public string? Description     { get; set; }
    public bool    IsSystem        { get; set; }
    public bool    IsActive        { get; set; }
    public int     PermissionCount { get; set; }
    public int     UserCount       { get; set; }
}

public class AdminRoleDetailsViewModel
{
    public int     Id          { get; set; }
    public string  RoleKey     { get; set; } = string.Empty;
    public string  RoleName    { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool    IsSystem    { get; set; }
    public bool    IsActive    { get; set; }
    public int     UserCount   { get; set; }

    public List<AdminPermissionGroupViewModel> PermissionGroups { get; set; } = new();
}

public class AdminRoleEditViewModel
{
    public int Id { get; set; }

    public string RoleKey  { get; set; } = string.Empty;
    public bool   IsSystem { get; set; }

    [Required, MaxLength(150), Display(Name = "Role Name")]
    public string RoleName { get; set; } = string.Empty;

    [MaxLength(500), Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class AdminRolePermissionsViewModel
{
    public int     RoleId   { get; set; }
    public string  RoleKey  { get; set; } = string.Empty;
    public string  RoleName { get; set; } = string.Empty;
    public bool    IsSystem { get; set; }

    public List<AdminPermissionGroupViewModel> Groups { get; set; } = new();
}

public class AdminPermissionGroupViewModel
{
    public string GroupName { get; set; } = string.Empty;
    public List<AdminPermissionSelectItemViewModel> Permissions { get; set; } = new();
}

public class AdminPermissionSelectItemViewModel
{
    public int    Id             { get; set; }
    public string PermissionKey  { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public bool   IsSelected     { get; set; }
}
