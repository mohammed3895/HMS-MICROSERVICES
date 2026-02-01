using HMS.Staff.Infrastructure.Data;
using HMS.Staff.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Infrastructure.Extensions
{
    public static class SeedExtensions
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<StaffDbContext>();
                var logger = services.GetRequiredService<ILogger<StaffDataSeeder>>();

                // Ensure database is created and migrated
                await context.Database.MigrateAsync();

                // Run seeder
                var seeder = new StaffDataSeeder(context, logger);
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}