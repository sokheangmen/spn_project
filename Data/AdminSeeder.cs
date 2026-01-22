using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using System;
using System.Threading.Tasks;

public static class AdminSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // ================= 1. Ensure Admin Role Exists =================
        var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.role_name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Roles
            {
                role_name = "Admin",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            db.Roles.Add(adminRole);
            await db.SaveChangesAsync(); // Save first to get role ID
        }

        // ================= 2. Ensure Admin User Exists =================
        var adminUser = await db.Users.FirstOrDefaultAsync(u => u.user_name == "admin");
        if (adminUser == null)
        {
            var hasher = new PasswordHasher<Users>();

            adminUser = new Users
            {
                user_name = "admin",
                email = "admin@gmail.com",
                password = hasher.HashPassword(null, "Admin@123"), // Can pass user object instead of null if you want
                is_active = true,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            db.Users.Add(adminUser);
            await db.SaveChangesAsync(); // Save first to get user ID
        }

        // ================= 3. Assign Admin Role to Admin User =================
        bool hasRole = await db.UserRoles.AnyAsync(x =>
            x.user_Id == adminUser.Id && x.role_Id == adminRole.Id);

        if (!hasRole)
        {
            db.UserRoles.Add(new UserRoles
            {
                user_Id = adminUser.Id,
                role_Id = adminRole.Id
            });

            await db.SaveChangesAsync();
        }
    }
}
