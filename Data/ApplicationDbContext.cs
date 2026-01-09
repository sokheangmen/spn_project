using Microsoft.EntityFrameworkCore;
using MyAPI.Models;

namespace MyAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= UserRole Composite Key =================
            modelBuilder.Entity<UserRoles>()
                .HasKey(ur => new { ur.user_Id, ur.role_Id });

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.Users)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.user_Id);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.role_Id);

            // ================= RolePermission Composite Key =================
            modelBuilder.Entity<RolePermissions>()
                .HasKey(rp => new { rp.role_Id, rp.permission_Id });

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.role_Id);

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.permission_Id);


            // ================= Seed Role =================
            //modelBuilder.Entity<Roles>().HasData(
            //    new Roles
            //    {
            //        Id = 1,
            //        role_name = "Admin"
            //    }
            //);
            modelBuilder.Entity<Users>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UsersProfile>(p => p.User_Id);

            // ================= Seed Admin User =================
            //string adminPasswordHash = Convert.ToBase64String(
            //    System.Security.Cryptography.SHA256.Create()
            //    .ComputeHash(System.Text.Encoding.UTF8.GetBytes("admin1234"))
            //);

            //modelBuilder.Entity<Users>().HasData(
            //    new Users
            //    {
            //        Id = 1,
            //        user_name = "admin",
            //        email = "admin@system.com",
            //        password = adminPasswordHash,
            //        is_active = true,
            //        created_at = new DateTime(2025, 1, 1) 
            //    }
            //);

            // ================= Assign Role to User =================
            //modelBuilder.Entity<UserRoles>().HasData(
            //    new UserRoles
            //    {
            //        user_Id = 1,
            //        role_Id = 1
            //    }
            //);
        }
    }
}
