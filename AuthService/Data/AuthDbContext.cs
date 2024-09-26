using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserID);
            modelBuilder.Entity<Role>().HasKey(r => r.RoleID);
            modelBuilder.Entity<Permission>().HasKey(p => p.PermissionID);
            modelBuilder.Entity<RolePermission>().HasKey(rp => rp.RolePermissionID);
            modelBuilder.Entity<UserRole>().HasKey(ur => ur.UserRoleID);
            modelBuilder.Entity<RefreshToken>().HasKey(rf => rf.Id);

            modelBuilder.Entity<RolePermission>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(rp => rp.RoleID);

            modelBuilder.Entity<RolePermission>()
                .HasOne<Permission>()
                .WithMany()
                .HasForeignKey(rp => rp.PermissionID);

            modelBuilder.Entity<UserRole>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ur => ur.UserID);

            modelBuilder.Entity<UserRole>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleID);

            modelBuilder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId);

            // Seed data for Users
            modelBuilder.Entity<User>().HasData(
                new User { UserID = 1, Username = "admin", PasswordHash = "hashed_password_1", Salt = "salt1", Email = "admin@example.com", FirstName = "Admin", LastName = "User", CreatedAt = DateTime.Now },
                new User { UserID = 2, Username = "john_doe", PasswordHash = "hashed_password_2", Salt = "salt2", Email = "john.doe@example.com", FirstName = "John", LastName = "Doe", CreatedAt = DateTime.Now },
                new User { UserID = 3, Username = "jane_smith", PasswordHash = "hashed_password_3", Salt = "salt3", Email = "jane.smith@example.com", FirstName = "Jane", LastName = "Smith", CreatedAt = DateTime.Now }
            );

            // Seed data for Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, RoleName = "Admin", Description = "Quản trị viên có toàn quyền" },
                new Role { RoleID = 2, RoleName = "User", Description = "Người dùng thông thường" }
            );

            // Seed data for Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionID = 1, PermissionName = "ViewUsers", Description = "Quyền xem người dùng" },
                new Permission { PermissionID = 2, PermissionName = "EditUsers", Description = "Quyền chỉnh sửa người dùng" },
                new Permission { PermissionID = 3, PermissionName = "DeleteUsers", Description = "Quyền xóa người dùng" }
            );

            // Seed data for UserRoles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserRoleID = 1, UserID = 1, RoleID = 1 }, // Admin
                new UserRole { UserRoleID = 2, UserID = 2, RoleID = 2 }, // John Doe
                new UserRole { UserRoleID = 3, UserID = 3, RoleID = 2 }  // Jane Smith
            );

            // Seed data for RolePermissions
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { RolePermissionID = 1, RoleID = 1, PermissionID = 1 }, // Admin can ViewUsers
                new RolePermission { RolePermissionID = 2, RoleID = 1, PermissionID = 2 }, // Admin can EditUsers
                new RolePermission { RolePermissionID = 3, RoleID = 1, PermissionID = 3 }, // Admin can DeleteUsers
                new RolePermission { RolePermissionID = 4, RoleID = 2, PermissionID = 1 }  // User can ViewUsers
            );

            // Seed data for RefreshTokens
            modelBuilder.Entity<RefreshToken>().HasData(
                new RefreshToken { Id = Guid.NewGuid(), JwtId = "test", UserId = 1, Token = "refresh_token_1", ExpiredAt = DateTime.Now.AddDays(30), IssuedAt = DateTime.Now, IsRevoked = false, IsUsed = false },
                new RefreshToken { Id = Guid.NewGuid(), JwtId = "test", UserId = 2, Token = "refresh_token_2", ExpiredAt = DateTime.Now.AddDays(30), IssuedAt = DateTime.Now, IsRevoked = false, IsUsed = false }
            );

            // Add other seed data if needed...
        }

    }
}
