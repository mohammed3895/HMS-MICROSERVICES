using HMS.Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Infrastructure.Data
{
    public class DatabaseInitializer
    {
        private readonly AuthenticationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(
            AuthenticationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Apply migrations
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied successfully");

                // Seed roles
                await SeedRolesAsync();

                // Seed default admin
                await SeedDefaultAdminAsync();

                _logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[]
            {
                new ApplicationRole
                {
                    Name = "Admin",
                    Description = "System administrator with full access",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationRole
                {
                    Name = "Manager",
                    Description = "Hospital manager with staff management access",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationRole
                {
                    Name = "Doctor",
                    Description = "Medical doctor",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationRole
                {
                    Name = "Nurse",
                    Description = "Nursing staff",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationRole
                {
                    Name = "Pharmacist",
                    Description = "Pharmacy staff",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationRole
                {
                    Name = "LabTechnician",
                    Description = "Laboratory technician",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationRole
                {
                    Name = "Receptionist",
                    Description = "Front desk receptionist",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationRole
                {
                    Name = "Patient",
                    Description = "Patient user",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name!))
                {
                    await _roleManager.CreateAsync(role);
                    _logger.LogInformation("Created role: {RoleName}", role.Name);
                }
            }

            // Seed role permissions
            await SeedRolePermissionsAsync();
        }

        private async Task SeedRolePermissionsAsync()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var permissions = new[]
                {
                    "users.create", "users.read", "users.update", "users.delete",
                    "roles.create", "roles.read", "roles.update", "roles.delete",
                    "staff.create", "staff.read", "staff.update", "staff.delete",
                    "patients.read", "patients.update",
                    "appointments.read", "appointments.update", "appointments.delete",
                    "reports.read", "reports.create",
                    "settings.read", "settings.update",
                    "audit.read"
                };

                foreach (var permission in permissions)
                {
                    var existingPermission = await _context.RolePermissions
                        .FirstOrDefaultAsync(rp => rp.RoleId == adminRole.Id && rp.Permission == permission);

                    if (existingPermission == null)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            Permission = permission,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            // Manager permissions
            var managerRole = await _roleManager.FindByNameAsync("Manager");
            if (managerRole != null)
            {
                var permissions = new[]
                {
                    "staff.create", "staff.read", "staff.update",
                    "patients.read", "patients.update",
                    "appointments.read", "appointments.update",
                    "reports.read", "reports.create"
                };

                foreach (var permission in permissions)
                {
                    var existingPermission = await _context.RolePermissions
                        .FirstOrDefaultAsync(rp => rp.RoleId == managerRole.Id && rp.Permission == permission);

                    if (existingPermission == null)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = managerRole.Id,
                            Permission = permission,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            // Doctor permissions
            var doctorRole = await _roleManager.FindByNameAsync("Doctor");
            if (doctorRole != null)
            {
                var permissions = new[]
                {
                    "patients.read", "patients.update",
                    "appointments.read", "appointments.create", "appointments.update",
                    "prescriptions.create", "prescriptions.read", "prescriptions.update",
                    "medical-records.create", "medical-records.read", "medical-records.update"
                };

                foreach (var permission in permissions)
                {
                    var existingPermission = await _context.RolePermissions
                        .FirstOrDefaultAsync(rp => rp.RoleId == doctorRole.Id && rp.Permission == permission);

                    if (existingPermission == null)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = doctorRole.Id,
                            Permission = permission,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedDefaultAdminAsync()
        {
            const string adminEmail = "admin@hms.com";
            const string adminPassword = "Admin@123456"; // Change this in production!

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    PhoneNumber = "+1234567890",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");

                    // Create admin profile
                    var adminProfile = new AdminProfile
                    {
                        UserId = adminUser.Id,
                        Department = "IT",
                        Position = "System Administrator",
                        JoiningDate = DateTime.UtcNow,
                        Permissions = new List<string> { "all" }
                    };

                    _context.AdminProfiles.Add(adminProfile);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Default admin user created: {Email}", adminEmail);
                    _logger.LogWarning("IMPORTANT: Change the default admin password immediately!");
                }
                else
                {
                    _logger.LogError("Failed to create default admin: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    // Extension method to easily call initializer
    public static class DatabaseInitializerExtensions
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await initializer.InitializeAsync();
        }
    }
}