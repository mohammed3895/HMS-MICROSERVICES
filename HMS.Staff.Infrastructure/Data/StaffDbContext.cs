using HMS.Staff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HMS.Staff.Infrastructure.Data
{
    public class StaffDbContext : DbContext
    {
        public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options) { }

        public DbSet<Domain.Entities.Staff> Staff { get; set; }
        public DbSet<StaffLeave> StaffLeaves { get; set; }
        public DbSet<StaffEducation> StaffEducations { get; set; }
        public DbSet<StaffExperience> StaffExperiences { get; set; }
        public DbSet<StaffCertification> StaffCertifications { get; set; }
        public DbSet<StaffAttendance> StaffAttendances { get; set; }
        public DbSet<StaffPerformanceReview> StaffPerformanceReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Domain.Entities.Staff>(entity =>
            {
                entity.ToTable("Staff");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StaffNumber).IsUnique();
                entity.HasIndex(e => e.Department);
                entity.HasIndex(e => e.StaffType);
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted });

                entity.Property(e => e.StaffNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BasicSalary).HasColumnType("decimal(18,2)");

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            builder.Entity<StaffLeave>(entity =>
            {
                entity.ToTable("StaffLeaves");
                entity.HasIndex(e => e.StaffId);
                entity.HasIndex(e => e.Status);
            });

            builder.Entity<StaffAttendance>(entity =>
            {
                entity.ToTable("StaffAttendances");
                entity.HasIndex(e => e.StaffId);
                entity.HasIndex(e => e.Date);
            });
        }
    }

    public class StaffDbContextFactory : IDesignTimeDbContextFactory<StaffDbContext>
    {
        public StaffDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HMS_MS_STAFF;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

            var optionsBuilder = new DbContextOptionsBuilder<StaffDbContext>();
            optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("HMS.Staff.Infrastructure"));

            return new StaffDbContext(optionsBuilder.Options);
        }
    }
}