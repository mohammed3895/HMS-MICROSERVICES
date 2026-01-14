using HMS.Staff.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Staff.Infrastructure.Data
{
    public class StaffDbContext : DbContext
    {
        public StaffDbContext(DbContextOptions<StaffDbContext> options)
            : base(options)
        {
        }

        public DbSet<HMS.Staff.Domain.Entities.Staff> StaffMembers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }
        public DbSet<StaffLeave> StaffLeaves { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<PatientAdmission> PatientAdmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Department Configuration
            builder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.PhoneExtension)
                    .HasMaxLength(20);

                entity.Property(e => e.Location)
                    .HasMaxLength(200);

                // Indexes
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.IsActive);

                // Self-referencing for Head of Department
                // HeadOfDepartmentId is a Staff member

                // One-to-Many with Staff
                entity.HasMany(e => e.StaffMembers)
                    .WithOne(e => e.Department)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // One-to-Many with Wards
                entity.HasMany(e => e.Wards)
                    .WithOne(e => e.Department)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Specialization Configuration
            builder.Entity<Specialization>(entity =>
            {
                entity.ToTable("Specializations");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Staff Configuration
            builder.Entity<HMS.Staff.Domain.Entities.Staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.EmployeeId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LicenseNumber)
                    .HasMaxLength(100);

                entity.Property(e => e.Qualifications)
                    .HasMaxLength(1000);

                entity.Property(e => e.ShiftPattern)
                    .HasMaxLength(200);

                entity.Property(e => e.HourlyRate)
                    .HasPrecision(18, 2);

                // Indexes
                entity.HasIndex(e => e.EmployeeId).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.SpecializationId);
                entity.HasIndex(e => e.StaffType);
                entity.HasIndex(e => e.Status);

                // NOTE: UserId references Authentication service
                // No navigation property across service boundary

                // Many-to-One with Department
                entity.HasOne(e => e.Department)
                    .WithMany(e => e.StaffMembers)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One with Specialization
                entity.HasOne(e => e.Specialization)
                    .WithMany()
                    .HasForeignKey(e => e.SpecializationId)
                    .OnDelete(DeleteBehavior.SetNull);

                // One-to-Many with StaffSchedule
                entity.HasMany(e => e.Schedules)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with StaffLeave
                entity.HasMany(e => e.Leaves)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(e => e.StaffId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // StaffSchedule Configuration
            builder.Entity<StaffSchedule>(entity =>
            {
                entity.ToTable("StaffSchedules");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);

                // Indexes
                entity.HasIndex(e => e.StaffId);
                entity.HasIndex(e => e.ScheduleDate);
                entity.HasIndex(e => new { e.StaffId, e.ScheduleDate });
                entity.HasIndex(e => e.IsActive);

                // Prevent overlapping schedules
                entity.HasIndex(e => new { e.StaffId, e.ScheduleDate, e.StartTime, e.EndTime })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");
            });

            // StaffLeave Configuration
            builder.Entity<StaffLeave>(entity =>
            {
                entity.ToTable("StaffLeaves");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Comments)
                    .HasMaxLength(2000);

                // Indexes
                entity.HasIndex(e => e.StaffId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.StaffId, e.StartDate, e.EndDate });
            });

            // Ward Configuration
            builder.Entity<Ward>(entity =>
            {
                entity.ToTable("Wards");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.WardType)
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneExtension)
                    .HasMaxLength(20);

                // Indexes
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.IsActive);

                // One-to-Many with Beds
                entity.HasMany(e => e.Beds)
                    .WithOne(e => e.Ward)
                    .HasForeignKey(e => e.WardId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with PatientAdmissions
                entity.HasMany(e => e.PatientAdmissions)
                    .WithOne(e => e.Ward)
                    .HasForeignKey(e => e.WardId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Bed Configuration
            builder.Entity<Bed>(entity =>
            {
                entity.ToTable("Beds");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.BedNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BedType)
                    .HasMaxLength(100);

                // Indexes
                entity.HasIndex(e => new { e.WardId, e.BedNumber }).IsUnique();
                entity.HasIndex(e => e.IsOccupied);
                entity.HasIndex(e => e.IsActive);

                // One-to-Many with PatientAdmissions
                entity.HasMany(e => e.PatientAdmissions)
                    .WithOne(e => e.Bed)
                    .HasForeignKey(e => e.BedId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PatientAdmission Configuration
            builder.Entity<PatientAdmission>(entity =>
            {
                entity.ToTable("PatientAdmissions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.AdmissionReason)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.DischargeNotes)
                    .HasMaxLength(2000);

                entity.Property(e => e.DischargeNotes)
                    .HasMaxLength(4000);

                // Indexes
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.WardId);
                entity.HasIndex(e => e.BedId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AdmissionDate);

                // NOTE: PatientId references Patient service
                // AttendingDoctorId references Staff within this service
            });

            // Add check constraints
            builder.Entity<StaffSchedule>()
                .HasCheckConstraint("CK_StaffSchedule_Time", "[EndTime] > [StartTime]");

            builder.Entity<StaffLeave>()
                .HasCheckConstraint("CK_StaffLeave_Date", "[EndDate] >= [StartDate]");

            builder.Entity<Ward>()
                .HasCheckConstraint("CK_Ward_Capacity", "[OccupiedBeds] <= [Capacity]");
        }
    }
}