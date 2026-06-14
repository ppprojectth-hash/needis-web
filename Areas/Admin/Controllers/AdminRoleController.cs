using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AdminRoleController : Controller
{
    private readonly AppDbContext _db;

    public AdminRoleController(AppDbContext db) => _db = db;

    // ── Index ────────────────────────────────────────────────────────────────

    [RequirePermission("Role.View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Roles & Permissions";

        var roles = await _db.AdminRoles.AsNoTracking()
            .Where(r => !r.IsDelete)
            .Select(r => new AdminRoleListItemViewModel
            {
                Id              = r.Id,
                RoleKey         = r.RoleKey,
                RoleName        = r.RoleName,
                Description     = r.Description,
                IsSystem        = r.IsSystem,
                IsActive        = r.IsActive,
                PermissionCount = r.RolePermissions.Count,
                UserCount       = r.AdminUsers.Count(u => u.IsActive),
            })
            .OrderBy(r => r.Id)
            .ToListAsync();

        return View(roles);
    }

    // ── Details ──────────────────────────────────────────────────────────────

    [RequirePermission("Role.View")]
    public async Task<IActionResult> Details(int id)
    {
        var role = await _db.AdminRoles.AsNoTracking()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.AdminPermission)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDelete);

        if (role is null) return NotFound();

        var assignedKeys = role.RolePermissions
            .Where(rp => rp.AdminPermission != null)
            .Select(rp => rp.AdminPermission!.PermissionKey)
            .ToHashSet();

        var allPerms = await _db.AdminPermissions.AsNoTracking()
            .Where(p => p.IsActive && !p.IsDelete)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

        var vm = new AdminRoleDetailsViewModel
        {
            Id          = role.Id,
            RoleKey     = role.RoleKey,
            RoleName    = role.RoleName,
            Description = role.Description,
            IsSystem    = role.IsSystem,
            IsActive    = role.IsActive,
            UserCount   = await _db.AdminUsers.CountAsync(u => u.AdminRoleId == role.Id && u.IsActive),
            PermissionGroups = allPerms
                .GroupBy(p => p.PermissionGroup)
                .Select(g => new AdminPermissionGroupViewModel
                {
                    GroupName = g.Key,
                    Permissions = g.Select(p => new AdminPermissionSelectItemViewModel
                    {
                        Id             = p.Id,
                        PermissionKey  = p.PermissionKey,
                        PermissionName = p.PermissionName,
                        IsSelected     = assignedKeys.Contains(p.PermissionKey),
                    }).ToList(),
                })
                .ToList(),
        };

        return View(vm);
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Role.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _db.AdminRoles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDelete);

        if (role is null) return NotFound();

        return View(new AdminRoleEditViewModel
        {
            Id          = role.Id,
            RoleKey     = role.RoleKey,
            RoleName    = role.RoleName,
            Description = role.Description,
            IsSystem    = role.IsSystem,
            IsActive    = role.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission("Role.Edit")]
    public async Task<IActionResult> Edit(int id, AdminRoleEditViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var role = await _db.AdminRoles.FindAsync(id);
        if (role is null || role.IsDelete) return NotFound();

        // Preserve system role key
        role.RoleName    = vm.RoleName;
        role.Description = vm.Description;
        role.IsActive    = vm.IsActive;
        role.UpdatedAt   = DateTime.UtcNow;
        role.UpdatedBy   = User.Identity?.Name;

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Role '{role.RoleName}' updated.";
        return RedirectToAction(nameof(Index));
    }

    // ── Permissions ──────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Role.View")]
    public async Task<IActionResult> Permissions(int id)
    {
        var role = await _db.AdminRoles.AsNoTracking()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDelete);

        if (role is null) return NotFound();

        var assignedIds = role.RolePermissions
            .Select(rp => rp.AdminPermissionId)
            .ToHashSet();

        var allPerms = await _db.AdminPermissions.AsNoTracking()
            .Where(p => p.IsActive && !p.IsDelete)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

        var vm = new AdminRolePermissionsViewModel
        {
            RoleId   = role.Id,
            RoleKey  = role.RoleKey,
            RoleName = role.RoleName,
            IsSystem = role.IsSystem,
            Groups   = allPerms
                .GroupBy(p => p.PermissionGroup)
                .Select(g => new AdminPermissionGroupViewModel
                {
                    GroupName = g.Key,
                    Permissions = g.Select(p => new AdminPermissionSelectItemViewModel
                    {
                        Id             = p.Id,
                        PermissionKey  = p.PermissionKey,
                        PermissionName = p.PermissionName,
                        IsSelected     = assignedIds.Contains(p.Id),
                    }).ToList(),
                })
                .ToList(),
        };

        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission("Role.Edit")]
    public async Task<IActionResult> UpdatePermissions(int id, int[]? selectedPermissionIds)
    {
        var role = await _db.AdminRoles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDelete);

        if (role is null) return NotFound();

        selectedPermissionIds ??= [];

        // SuperAdmin must always keep all permissions
        if (role.RoleKey == "SuperAdmin")
        {
            TempData["Warning"] = "SuperAdmin permissions are managed automatically and cannot be changed here.";
            return RedirectToAction(nameof(Permissions), new { id });
        }

        // Remove old, add new
        _db.AdminRolePermissions.RemoveRange(role.RolePermissions);
        await _db.SaveChangesAsync();

        var newPerms = selectedPermissionIds
            .Select(permId => new AdminRolePermission
            {
                AdminRoleId       = id,
                AdminPermissionId = permId,
                CreatedAt         = DateTime.UtcNow,
                CreatedBy         = User.Identity?.Name,
            })
            .ToList();

        _db.AdminRolePermissions.AddRange(newPerms);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Permissions for '{role.RoleName}' updated. {newPerms.Count} permission(s) assigned.";
        return RedirectToAction(nameof(Permissions), new { id });
    }
}
