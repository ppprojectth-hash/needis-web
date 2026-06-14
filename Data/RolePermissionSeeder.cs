using Microsoft.EntityFrameworkCore;
using Needis.Web.Models;

namespace Needis.Web.Data;

public static class RolePermissionSeeder
{
    // ── All permission definitions ────────────────────────────────────────────
    private static readonly (string Key, string Group, string Name, int Order)[] Permissions =
    [
        ("Dashboard.View",       "Dashboard", "View Dashboard",           1),

        ("AdminUser.View",       "System",    "View Admin Users",         10),
        ("AdminUser.Create",     "System",    "Create Admin Users",       11),
        ("AdminUser.Edit",       "System",    "Edit Admin Users",         12),
        ("AdminUser.Delete",     "System",    "Delete Admin Users",       13),
        ("Role.View",            "System",    "View Roles & Permissions", 14),
        ("Role.Edit",            "System",    "Edit Roles & Permissions", 15),
        ("SiteSetting.View",     "System",    "View Site Settings",       16),
        ("SiteSetting.Edit",     "System",    "Edit Site Settings",       17),
        ("UsageStatistic.View",  "System",    "View Usage Statistics",    18),

        ("Banner.View",          "Content",   "View Banners",             20),
        ("Banner.Create",        "Content",   "Create Banners",           21),
        ("Banner.Edit",          "Content",   "Edit Banners",             22),
        ("Banner.Delete",        "Content",   "Delete Banners",           23),
        ("Category.View",        "Content",   "View Categories",          24),
        ("Category.Create",      "Content",   "Create Categories",        25),
        ("Category.Edit",        "Content",   "Edit Categories",          26),
        ("Category.Delete",      "Content",   "Delete Categories",        27),
        ("Product.View",         "Content",   "View Products",            28),
        ("Product.Create",       "Content",   "Create Products",          29),
        ("Product.Edit",         "Content",   "Edit Products",            30),
        ("Product.Delete",       "Content",   "Delete Products",          31),

        ("About.View",           "About",     "View About Us",            40),
        ("About.Edit",           "About",     "Edit About Us",            41),

        ("Service.View",         "Services",  "View Services",            50),
        ("Service.Edit",         "Services",  "Edit Services",            51),

        ("Activity.View",        "Activity",  "View Activities",          60),
        ("Activity.Edit",        "Activity",  "Edit Activities",          61),

        ("Quotation.View",       "Sales",     "View Quotation Requests",  70),
        ("Quotation.Edit",       "Sales",     "Edit Quotation Requests",  71),
        ("ContactMessage.View",  "Sales",     "View Contact Messages",    72),
        ("ContactMessage.Edit",  "Sales",     "Edit Contact Messages",    73),
        ("EmailLog.View",        "Sales",     "View Email Logs",          74),

        ("Media.View",           "Media",     "View Media Library",       80),
        ("Media.Upload",         "Media",     "Upload Media",             81),
        ("Media.Edit",           "Media",     "Edit Media Metadata",      82),
        ("Media.Delete",         "Media",     "Delete Media",             83),

        ("Seo.View",             "SEO",       "View SEO Settings",        90),
        ("Seo.Edit",             "SEO",       "Edit SEO Settings",        91),

        ("Manual.View",          "System",    "View Manuals",             92),
    ];

    private static readonly string[] AllKeys =
        Permissions.Select(p => p.Key).ToArray();

    // ── Role definitions with their default permission sets ───────────────────
    private static readonly (string Key, string Name, string? Desc, bool IsSystem, string[] PermKeys)[] Roles =
    [
        ("SuperAdmin", "Super Administrator", "Full access to everything", true,
            AllKeys),

        ("Admin", "Administrator", "Manage most content", true,
            AllKeys.Where(k => k != "Role.Edit" && k != "AdminUser.Delete").ToArray()),

        ("Editor", "Content Editor", "Manage public content", true,
        [
            "Dashboard.View",
            "SiteSetting.View",
            "Banner.View",    "Banner.Create",    "Banner.Edit",    "Banner.Delete",
            "Category.View",  "Category.Create",  "Category.Edit",  "Category.Delete",
            "Product.View",   "Product.Create",   "Product.Edit",   "Product.Delete",
            "About.View",     "About.Edit",
            "Service.View",   "Service.Edit",
            "Activity.View",  "Activity.Edit",
            "Media.View",     "Media.Upload",   "Media.Edit",
            "Seo.View",       "Seo.Edit",
            "Manual.View",
        ]),

        ("Sales", "Sales Staff", "Manage quotations and messages", true,
        [
            "Dashboard.View",
            "Product.View", "Service.View", "Activity.View",
            "Quotation.View",      "Quotation.Edit",
            "ContactMessage.View", "ContactMessage.Edit",
            "EmailLog.View",
            "Manual.View",
        ]),

        ("Viewer", "Viewer", "Read-only access to dashboard and reports", true,
        [
            "Dashboard.View",
            "Product.View", "Category.View",
            "About.View",   "Service.View",  "Activity.View",
            "Quotation.View", "ContactMessage.View",
            "UsageStatistic.View", "EmailLog.View",
            "Media.View",
            "Seo.View",
            "Manual.View",
        ]),
    ];

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // ── Seed missing permissions ──────────────────────────────────────────
        var existingKeys = (await db.AdminPermissions.AsNoTracking()
            .Select(p => p.PermissionKey)
            .ToListAsync())
            .ToHashSet();

        var toAdd = Permissions
            .Where(p => !existingKeys.Contains(p.Key))
            .Select(p => new AdminPermission
            {
                PermissionKey   = p.Key,
                PermissionGroup = p.Group,
                PermissionName  = p.Name,
                DisplayOrder    = p.Order,
                IsActive        = true,
                IsDelete        = false,
                CreatedAt       = DateTime.UtcNow,
                CreatedBy       = "Seeder",
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.AdminPermissions.AddRange(toAdd);
            await db.SaveChangesAsync();
        }

        // Reload full permission dictionary
        var permDict = await db.AdminPermissions.AsNoTracking()
            .Where(p => !p.IsDelete)
            .ToDictionaryAsync(p => p.PermissionKey, p => p.Id);

        // ── Seed roles and their permissions ─────────────────────────────────
        foreach (var (roleKey, roleName, desc, isSystem, permKeys) in Roles)
        {
            var role = await db.AdminRoles.FirstOrDefaultAsync(r => r.RoleKey == roleKey);
            if (role is null)
            {
                role = new AdminRole
                {
                    RoleKey     = roleKey,
                    RoleName    = roleName,
                    Description = desc,
                    IsSystem    = isSystem,
                    IsActive    = true,
                    IsDelete    = false,
                    CreatedAt   = DateTime.UtcNow,
                    CreatedBy   = "Seeder",
                };
                db.AdminRoles.Add(role);
                await db.SaveChangesAsync();
            }

            // Only seed permissions if none exist yet (preserves custom edits)
            var hasPerms = await db.AdminRolePermissions.AnyAsync(rp => rp.AdminRoleId == role.Id);
            if (!hasPerms)
            {
                var rolePerms = permKeys
                    .Where(k => permDict.ContainsKey(k))
                    .Select(k => new AdminRolePermission
                    {
                        AdminRoleId       = role.Id,
                        AdminPermissionId = permDict[k],
                        CreatedAt         = DateTime.UtcNow,
                        CreatedBy         = "Seeder",
                    })
                    .ToList();

                db.AdminRolePermissions.AddRange(rolePerms);
                await db.SaveChangesAsync();
            }
        }

        // ── Link existing SuperAdmin users to the SuperAdmin role ─────────────
        var superAdminRole = await db.AdminRoles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoleKey == "SuperAdmin");

        if (superAdminRole is not null)
        {
            var unlinked = await db.AdminUsers
                .Where(u => u.Role == "SuperAdmin" && u.AdminRoleId == null)
                .ToListAsync();

            foreach (var u in unlinked)
                u.AdminRoleId = superAdminRole.Id;

            if (unlinked.Count > 0)
                await db.SaveChangesAsync();
        }
    }
}
