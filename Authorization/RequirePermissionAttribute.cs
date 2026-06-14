using Microsoft.AspNetCore.Authorization;

namespace Needis.Web.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionKey)
        => Policy = $"Permission:{permissionKey}";
}
