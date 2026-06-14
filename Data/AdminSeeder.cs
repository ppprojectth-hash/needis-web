using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Models;

namespace Needis.Web.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (await db.AdminUsers.AnyAsync())
            return;

        var seed = config.GetSection("AdminSeed");
        var password = seed["Password"]
            ?? throw new InvalidOperationException("AdminSeed:Password is not configured.");

        var user = new AdminUser
        {
            Username    = seed["Username"]  ?? "admin",
            Email       = seed["Email"]     ?? "admin@needis.local",
            DisplayName = seed["FullName"]  ?? "System Administrator",
            Role        = seed["Role"]      ?? "SuperAdmin",
            IsActive    = true,
            CreatedAt   = DateTime.UtcNow,
        };

        user.PasswordHash = new PasswordHasher<AdminUser>().HashPassword(user, password);

        db.AdminUsers.Add(user);
        await db.SaveChangesAsync();
    }
}
