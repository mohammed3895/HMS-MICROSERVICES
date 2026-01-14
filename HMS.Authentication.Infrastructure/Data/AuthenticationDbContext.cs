using HMS.Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Infrastructure.Data
{
    public class AuthenticationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Remove UserRoleAssignments DbSet - we'll use Identity's default UserRoles

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure table names
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            // ApplicationUser Configuration
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NationalId).HasMaxLength(50);
                entity.Property(e => e.LicenseNumber).HasMaxLength(50);
                entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);

                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.NationalId).IsUnique();

                // One-to-One with UserProfile
                entity.HasOne(e => e.Profile)
                    .WithOne(e => e.User)
                    .HasForeignKey<UserProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with LoginHistory
                entity.HasMany(e => e.LoginHistories)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Note: UserRoles is managed by Identity, no need to configure
            });

            // ApplicationRole Configuration
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);

                // One-to-Many with RolePermission
                entity.HasMany(e => e.RolePermissions)
                    .WithOne(e => e.Role)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Note: UserRoles is managed by Identity, no need to configure
            });

            // UserProfile Configuration
            builder.Entity<UserProfile>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(100);
                entity.Property(e => e.ZipCode).HasMaxLength(20);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.EmergencyContactName).HasMaxLength(200);
                entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
                entity.Property(e => e.BloodGroup).HasMaxLength(10);
                entity.Property(e => e.Allergies).HasMaxLength(1000);
            });

            // RolePermission Configuration
            builder.Entity<RolePermission>(entity =>
            {
                entity.Property(e => e.Permission).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => new { e.RoleId, e.Permission }).IsUnique();
            });

            // LoginHistory Configuration
            builder.Entity<LoginHistory>(entity =>
            {
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.FailureReason).HasMaxLength(500);
                entity.Property(e => e.DeviceId).HasMaxLength(200);

                entity.HasIndex(e => e.LoginTime);
                entity.HasIndex(e => new { e.UserId, e.LoginTime });
            });

            // OtpVerification Configuration
            builder.Entity<OtpVerification>(entity =>
            {
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.OtpCode).IsRequired().HasMaxLength(10);

                entity.HasIndex(e => new { e.Email, e.OtpCode, e.IsUsed });
                entity.HasIndex(e => e.ExpiresAt);
            });

            // AuditLog Configuration
            builder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username)
                    .HasMaxLength(256);

                entity.Property(e => e.EntityName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.EntityId)
                    .HasMaxLength(50);

                entity.Property(e => e.OldValues)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.NewValues)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Changes)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50);

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(500);

                entity.Property(e => e.AdditionalInfo)
                    .HasMaxLength(2000);

                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(2000);

                // Indexes for common queries
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Action);
                entity.HasIndex(e => e.EntityName);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => new { e.EntityName, e.EntityId });
                entity.HasIndex(e => new { e.UserId, e.Timestamp });
                entity.HasIndex(e => new { e.Action, e.Timestamp });
            });
        }
    }
}