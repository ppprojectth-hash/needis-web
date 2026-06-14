using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;

namespace Needis.Web.Services.Permissions;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _db;

    // Per-request cache (service is Scoped)
    private int?          _cachedUserId;
    private List<string>? _cachedPermissions;

    public PermissionService(AppDbContext db) => _db = db;

    public bool IsSuperAdmin(ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Role) == "SuperAdmin";

    public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permissionKey)
    {
        if (user.Identity?.IsAuthenticated != true) return false;
        if (IsSuperAdmin(user)) return true;

        var permissions = await GetUserPermissionsAsync(user);
        return permissions.Contains(permissionKey);
    }

    public async Task<List<string>> GetUserPermissionsAsync(ClaimsPrincipal user)
    {
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return [];

        if (_cachedUserId == userId && _cachedPermissions is not null)
            return _cachedPermissions;

        _cachedUserId = userId;

        var adminUser = await _db.AdminUsers.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (adminUser is null)
        {
            _cachedPermissions = [];
            return _cachedPermissions;
        }

        // Prefer linked AdminRoleId; fall back to Role string lookup
        int? roleId = adminUser.AdminRoleId;
        if (roleId is null && !string.IsNullOrEmpty(adminUser.Role))
        {
            var fallback = await _db.AdminRoles.AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleKey == adminUser.Role && r.IsActive && !r.IsDelete);
            roleId = fallback?.Id;
        }

        if (roleId is null)
        {
            _cachedPermissions = [];
            return _cachedPermissions;
        }

        _cachedPermissions = await _db.AdminRolePermissions
            .AsNoTracking()
            .Where(rp => rp.AdminRoleId == roleId)
            .Join(_db.AdminPermissions,
                  rp => rp.AdminPermissionId,
                  p  => p.Id,
                  (rp, p) => new { p.PermissionKey, p.IsActive, p.IsDelete })
            .Where(x => x.IsActive && !x.IsDelete)
            .Select(x => x.PermissionKey)
            .ToListAsync();

        return _cachedPermissions;
    }
}
