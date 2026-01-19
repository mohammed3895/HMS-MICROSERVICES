using HMS.Authentication.Domain.DTOs.Settings;
using HMS.Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HMS.Authentication.Infrastructure.Data
{
    public class AuthenticationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {
        }

        // Existing DbSets
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }

        // New DbSets for role-specific profiles
        public DbSet<DoctorProfile> DoctorProfiles { get; set; }
        public DbSet<NurseProfile> NurseProfiles { get; set; }
        public DbSet<PharmacistProfile> PharmacistProfiles { get; set; }
        public DbSet<LabTechnicianProfile> LabTechnicianProfiles { get; set; }
        public DbSet<ReceptionistProfile> ReceptionistProfiles { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<SecurityEvent> SecurityEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser Configuration
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.NationalId).IsUnique().HasFilter("[NationalId] IS NOT NULL");
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.RefreshToken).HasMaxLength(500);
                entity.Property(e => e.TwoFactorSecret).HasMaxLength(500);

                entity.HasOne(e => e.Profile)
                    .WithOne(e => e.User)
                    .HasForeignKey<UserProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Devices)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.WebAuthnCredentials)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.AuditLogs)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.PasswordResetTokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ApplicationRole Configuration
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasMany(e => e.Permissions)
                    .WithOne(e => e.Role)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserProfile Configuration
            builder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfiles");
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(100);
                entity.Property(e => e.ZipCode).HasMaxLength(20);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.BloodGroup).HasMaxLength(10);
            });

            // UserSettings Configuration
            builder.Entity<UserSettings>(entity =>
            {
                entity.ToTable("UserSettings");
                entity.HasIndex(e => e.UserId).IsUnique();

                // Configure JSON columns for complex types
                entity.Property(e => e.NotificationSettings)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<NotificationSettingsDto>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new NotificationSettingsDto()
                    )
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.PrivacySettings)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<PrivacySettingsDto>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new PrivacySettingsDto()
                    )
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Preferences)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<PreferencesDto>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new PreferencesDto()
                    )
                    .HasColumnType("nvarchar(max)");

                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<UserSettings>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Doctor Profile Configuration
            builder.Entity<DoctorProfile>(entity =>
            {
                entity.ToTable("DoctorProfiles");
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.MedicalLicenseNumber).IsUnique();
                entity.Property(e => e.MedicalLicenseNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Specialization).HasMaxLength(100);
                entity.Property(e => e.EmployeeId).HasMaxLength(50);
                entity.Property(e => e.Qualification).HasMaxLength(500);
                entity.Property(e => e.ConsultationFee).HasMaxLength(50);
                entity.Property(e => e.Biography).HasMaxLength(2000);
            });

            // Nurse Profile Configuration
            builder.Entity<NurseProfile>(entity =>
            {
                entity.ToTable("NurseProfiles");
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.NursingLicenseNumber).IsUnique();
                entity.Property(e => e.NursingLicenseNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Shift).HasMaxLength(50);
                entity.Property(e => e.Ward).HasMaxLength(100);
                entity.Property(e => e.EmployeeId).HasMaxLength(50);
            });

            // Pharmacist Profile Configuration
            builder.Entity<PharmacistProfile>(entity =>
            {
                entity.ToTable("PharmacistProfiles");
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.PharmacyLicenseNumber).IsUnique();
                entity.Property(e => e.PharmacyLicenseNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.PharmacyLocation).HasMaxLength(200);
                entity.Property(e => e.EmployeeId).HasMaxLength(50);
            });

            // Lab Technician Profile Configuration
            builder.Entity<LabTechnicianProfile>(entity =>
            {
                entity.ToTable("LabTechnicianProfiles");
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.LabSection).HasMaxLength(100);
                entity.Property(e => e.EmployeeId).HasMaxLength(50);
            });

            // Receptionist Profile Configuration
            builder.Entity<ReceptionistProfile>(entity =>
            {
                entity.ToTable("ReceptionistProfiles");
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Shift).HasMaxLength(50);
                entity.Property(e => e.Desk).HasMaxLength(50);
                entity.Property(e => e.EmployeeId).HasMaxLength(50);
            });

            // Admin Profile Configuration
            builder.Entity<AdminProfile>(entity =>
            {
                entity.ToTable("AdminProfiles");
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Position).HasMaxLength(100);
            });

            // UserDevice Configuration
            builder.Entity<UserDevice>(entity =>
            {
                entity.ToTable("UserDevices");
                entity.HasIndex(e => new { e.UserId, e.DeviceId });
                entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DeviceName).HasMaxLength(200);
                entity.Property(e => e.DeviceType).HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
            });

            // UserCredential Configuration
            builder.Entity<UserCredential>(entity =>
            {
                entity.ToTable("UserCredentials");
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.CredentialId).IsRequired();
                entity.Property(e => e.PublicKey).IsRequired();
                entity.Property(e => e.CredType).HasMaxLength(50);
            });

            // PasswordResetToken Configuration
            builder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetTokens");
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            });

            // AuditLog Configuration
            builder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Timestamp);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityName).HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
            });

            // RolePermission Configuration
            builder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasIndex(e => new { e.RoleId, e.Permission }).IsUnique();
                entity.Property(e => e.Permission).IsRequired().HasMaxLength(100);
            });

            // LoginHistory Configuration
            builder.Entity<LoginHistory>(entity =>
            {
                entity.ToTable("LoginHistories");
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.LoginTime);
                entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UserAgent).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FailureReason).HasMaxLength(500);
                entity.Property(e => e.DeviceId).HasMaxLength(200);
                entity.Property(e => e.LoginMethod).HasMaxLength(50);
                entity.Property(e => e.Location).HasMaxLength(200);
            });

            // OtpVerification Configuration
            builder.Entity<OtpVerification>(entity =>
            {
                entity.ToTable("OtpVerifications");
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.CreatedAt);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.OtpCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Purpose).HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
            });

            // UserSession Configuration
            builder.Entity<UserSession>(entity =>
            {
                entity.ToTable("UserSessions");
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.SessionToken).IsUnique();
                entity.HasIndex(e => e.RefreshToken).IsUnique();
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.ExpiresAt);
                entity.Property(e => e.SessionToken).IsRequired().HasMaxLength(500);
                entity.Property(e => e.RefreshToken).IsRequired().HasMaxLength(500);
                entity.Property(e => e.DeviceId).HasMaxLength(200);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.RevokeReason).HasMaxLength(500);
            });

            // SecurityEvent Configuration
            builder.Entity<SecurityEvent>(entity =>
            {
                entity.ToTable("SecurityEvents");
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.OccurredAt);
                entity.HasIndex(e => e.Severity);
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Severity).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.DeviceId).HasMaxLength(200);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.Resolution).HasMaxLength(1000);
            });
        }
    }

    // ==================== FIXED DbContext Factory ====================
    public class AuthenticationDbContextFactory : IDesignTimeDbContextFactory<AuthenticationDbContext>
    {
        public AuthenticationDbContext CreateDbContext(string[] args)
        {
            // Get the current directory (Infrastructure project's bin folder)
            var currentDirectory = Directory.GetCurrentDirectory();

            // Navigate to the API project directory
            // Assuming structure: Infrastructure/bin/Debug/net9.0 -> back to Infrastructure -> to API
            var infrastructureDir = Directory.GetParent(currentDirectory);
            while (infrastructureDir != null && !infrastructureDir.Name.Contains("Infrastructure"))
            {
                infrastructureDir = infrastructureDir.Parent;
            }

            // Go up one level to Services/Authentication folder, then to API
            string? apiProjectPath = null;
            if (infrastructureDir?.Parent != null)
            {
                // Look for HMS.Authentication.API directory
                var apiDir = Path.Combine(infrastructureDir.Parent.FullName, "HMS.Authentication.API");
                if (Directory.Exists(apiDir))
                {
                    apiProjectPath = apiDir;
                }
            }

            // If we still can't find it, try a different approach
            if (apiProjectPath == null || !Directory.Exists(apiProjectPath))
            {
                // Try to find it relative to current execution directory
                var parentDir = Directory.GetParent(currentDirectory);
                while (parentDir != null)
                {
                    var possibleApiPath = Path.Combine(parentDir.FullName, "HMS.Authentication.API");
                    if (Directory.Exists(possibleApiPath))
                    {
                        apiProjectPath = possibleApiPath;
                        break;
                    }
                    parentDir = parentDir.Parent;
                }
            }

            // Fallback: Use connection string directly if appsettings.json not found
            if (apiProjectPath == null || !File.Exists(Path.Combine(apiProjectPath, "appsettings.json")))
            {
                Console.WriteLine("⚠️  Warning: Could not locate appsettings.json. Using default connection string.");
                Console.WriteLine($"Current directory: {currentDirectory}");

                // Use the connection string from your appsettings.json
                var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HMS_MS_AUTH;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

                var builder = new DbContextOptionsBuilder<AuthenticationDbContext>();
                builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("HMS.Authentication.Infrastructure"));

                return new AuthenticationDbContext(builder.Options);
            }

            // Load configuration from API project
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString2 = configuration.GetConnectionString("AuthenticationDb");

            if (string.IsNullOrEmpty(connectionString2))
            {
                throw new InvalidOperationException("Connection string 'AuthenticationDb' not found in appsettings.json");
            }

            var optionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContext>();
            optionsBuilder.UseSqlServer(connectionString2, b => b.MigrationsAssembly("HMS.Authentication.Infrastructure"));

            Console.WriteLine($"✅ Using connection string from: {apiProjectPath}");
            return new AuthenticationDbContext(optionsBuilder.Options);
        }
    }
}