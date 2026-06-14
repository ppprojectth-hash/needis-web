using System.Security.Claims;

namespace Needis.Web.Services.Permissions;

public interface IPermissionService
{
    bool IsSuperAdmin(ClaimsPrincipal user);
    Task<bool>         HasPermissionAsync(ClaimsPrincipal user, string permissionKey);
    Task<List<string>> GetUserPermissionsAsync(ClaimsPrincipal user);
}
