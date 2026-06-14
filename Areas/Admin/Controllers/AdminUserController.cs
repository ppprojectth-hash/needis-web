using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AdminUserController : Controller
{
    private readonly AppDbContext               _db;
    private readonly IPasswordHasher<AdminUser> _hasher;

    public AdminUserController(AppDbContext db, IPasswordHasher<AdminUser> hasher)
    {
        _db     = db;
        _hasher = hasher;
    }

    private async Task PopulateRolesAsync(int? selectedRoleId = null)
    {
        var roles = await _db.AdminRoles.AsNoTracking()
            .Where(r => r.IsActive && !r.IsDelete)
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, r.RoleKey, r.RoleName })
            .ToListAsync();

        ViewBag.AdminRoles = new SelectList(roles, "Id", "RoleName", selectedRoleId);
    }

    // ── GET Index ────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("AdminUser.View")]
    public async Task<IActionResult> Index()
    {
        var users = await _db.AdminUsers.AsNoTracking()
            .Include(u => u.AdminRole)
            .OrderBy(u => u.Username)
            .Select(u => new AdminUserListItemViewModel
            {
                Id          = u.Id,
                Username    = u.Username,
                Email       = u.Email,
                DisplayName = u.DisplayName,
                Role        = u.AdminRole != null ? u.AdminRole.RoleName : u.Role,
                IsActive    = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                CreatedAt   = u.CreatedAt,
            })
            .ToListAsync();

        return View(users);
    }

    // ── GET Create ───────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("AdminUser.Create")]
    public async Task<IActionResult> Create()
    {
        await PopulateRolesAsync();
        return View(new AdminUserCreateViewModel());
    }

    // ── POST Create ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("AdminUser.Create")]
    public async Task<IActionResult> Create(AdminUserCreateViewModel vm)
    {
        if (!ModelState.IsValid) { await PopulateRolesAsync(vm.AdminRoleId); return View(vm); }

        if (await _db.AdminUsers.AnyAsync(u => u.Username == vm.Username))
        {
            ModelState.AddModelError(nameof(vm.Username), "Username already exists.");
            await PopulateRolesAsync(vm.AdminRoleId); return View(vm);
        }

        if (await _db.AdminUsers.AnyAsync(u => u.Email == vm.Email))
        {
            ModelState.AddModelError(nameof(vm.Email), "Email already exists.");
            await PopulateRolesAsync(vm.AdminRoleId); return View(vm);
        }

        // Resolve role string from AdminRoleId
        string roleStr = vm.Role;
        if (vm.AdminRoleId.HasValue)
        {
            var role = await _db.AdminRoles.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == vm.AdminRoleId.Value);
            if (role != null) roleStr = role.RoleKey;
        }

        // Non-SuperAdmin cannot create SuperAdmin users
        var currentRole = User.FindFirstValue(ClaimTypes.Role);
        if (roleStr == "SuperAdmin" && currentRole != "SuperAdmin")
        {
            ModelState.AddModelError(string.Empty, "Only SuperAdmin can create SuperAdmin users.");
            await PopulateRolesAsync(vm.AdminRoleId); return View(vm);
        }

        var user = new AdminUser
        {
            Username    = vm.Username,
            Email       = vm.Email,
            DisplayName = !string.IsNullOrWhiteSpace(vm.DisplayName) ? vm.DisplayName : vm.Username,
            Role        = roleStr,
            AdminRoleId = vm.AdminRoleId,
            IsActive    = vm.IsActive,
            CreatedAt   = DateTime.UtcNow,
        };
        user.PasswordHash = _hasher.HashPassword(user, vm.Password);

        _db.AdminUsers.Add(user);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"User '{user.Username}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── GET Edit ─────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("AdminUser.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _db.AdminUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        await PopulateRolesAsync(user.AdminRoleId);
        return View(new AdminUserEditViewModel
        {
            Id          = user.Id,
            Username    = user.Username,
            Email       = user.Email,
            DisplayName = user.DisplayName,
            Role        = user.Role,
            AdminRoleId = user.AdminRoleId,
            IsActive    = user.IsActive,
        });
    }

    // ── POST Edit ────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("AdminUser.Edit")]
    public async Task<IActionResult> Edit(int id, AdminUserEditViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) { await PopulateRolesAsync(vm.AdminRoleId); return View(vm); }

        if (await _db.AdminUsers.AnyAsync(u => u.Username == vm.Username && u.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Username), "Username already taken by another user.");
            await PopulateRolesAsync(vm.AdminRoleId); return View(vm);
        }

        if (await _db.AdminUsers.AnyAsync(u => u.Email == vm.Email && u.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Email), "Email already taken by another user.");
            await PopulateRolesAsync(vm.AdminRoleId); return View(vm);
        }

        var user = await _db.AdminUsers.FindAsync(id);
        if (user is null) return NotFound();

        // Resolve role from AdminRoleId
        string roleStr = vm.Role;
        if (vm.AdminRoleId.HasValue)
        {
            var role = await _db.AdminRoles.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == vm.AdminRoleId.Value);
            if (role != null) roleStr = role.RoleKey;
        }

        // Non-SuperAdmin cannot assign SuperAdmin role
        var currentRole = User.FindFirstValue(ClaimTypes.Role);
        if (roleStr == "SuperAdmin" && currentRole != "SuperAdmin")
        {
            ModelState.AddModelError(string.Empty, "Only SuperAdmin can assign the SuperAdmin role.");
            await PopulateRolesAsync(vm.AdminRoleId); return View(vm);
        }

        // Prevent removing own SuperAdmin role
        var currentUserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var cuid) ? cuid : 0;
        if (user.Id == currentUserId && user.Role == "SuperAdmin" && roleStr != "SuperAdmin")
        {
            TempData["Error"] = "You cannot remove your own SuperAdmin role.";
            return RedirectToAction(nameof(Index));
        }

        user.Username    = vm.Username;
        user.Email       = vm.Email;
        user.DisplayName = !string.IsNullOrWhiteSpace(vm.DisplayName) ? vm.DisplayName : vm.Username;
        user.Role        = roleStr;
        user.AdminRoleId = vm.AdminRoleId;
        user.IsActive    = vm.IsActive;

        await _db.SaveChangesAsync();

        TempData["Success"] = $"User '{user.Username}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── GET ChangePassword ───────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("AdminUser.Edit")]
    public async Task<IActionResult> ChangePassword(int id)
    {
        var user = await _db.AdminUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        return View(new AdminUserChangePasswordViewModel { Id = user.Id, Username = user.Username });
    }

    // ── POST ChangePassword ──────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("AdminUser.Edit")]
    public async Task<IActionResult> ChangePassword(int id, AdminUserChangePasswordViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var user = await _db.AdminUsers.FindAsync(id);
        if (user is null) return NotFound();

        user.PasswordHash = _hasher.HashPassword(user, vm.NewPassword);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Password for '{user.Username}' changed successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── POST ToggleActive ────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("AdminUser.Edit")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var user = await _db.AdminUsers.FindAsync(id);
        if (user is null) return NotFound();

        var currentUserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;

        if (user.Id == currentUserId && user.IsActive)
        {
            TempData["Error"] = "You cannot deactivate your own account.";
            return RedirectToAction(nameof(Index));
        }

        if (user.Role == "SuperAdmin" && user.IsActive)
        {
            var activeSuperAdminCount = await _db.AdminUsers
                .CountAsync(u => u.Role == "SuperAdmin" && u.IsActive);

            if (activeSuperAdminCount <= 1)
            {
                TempData["Error"] = "Cannot deactivate the last active SuperAdmin account.";
                return RedirectToAction(nameof(Index));
            }
        }

        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync();

        TempData["Success"] = user.IsActive
            ? $"User '{user.Username}' has been activated."
            : $"User '{user.Username}' has been deactivated.";

        return RedirectToAction(nameof(Index));
    }
}
