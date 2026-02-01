using HMS.Appointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HMS.Appointment.Infrastructure.Data
{
    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
            : base(options)
        {
        }

        public DbSet<HMS.Appointment.Domain.Entities.Appointment> Appointments { get; set; }
        public DbSet<AppointmentHistory> AppointmentHistories { get; set; }
        public DbSet<AppointmentReminder> AppointmentReminders { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DoctorLeave> DoctorLeaves { get; set; }
        public DbSet<ScheduleException> ScheduleExceptions { get; set; }
        public DbSet<WaitlistEntry> WaitlistEntries { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<AppointmentConflict> AppointmentConflicts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Appointment Configuration
            builder.Entity<HMS.Appointment.Domain.Entities.Appointment>(entity =>
            {
                entity.ToTable("Appointments");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AppointmentNumber).IsUnique();
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.AppointmentDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.DoctorId, e.AppointmentDate, e.Status });

                entity.Property(e => e.AppointmentNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PatientName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PatientPhone).HasMaxLength(20);
                entity.Property(e => e.PatientEmail).HasMaxLength(256);
                entity.Property(e => e.DoctorName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Specialization).HasMaxLength(100);
                entity.Property(e => e.CancellationReason).HasMaxLength(500);
                entity.Property(e => e.CheckInMethod).HasMaxLength(50);
                entity.Property(e => e.ChiefComplaint).HasMaxLength(1000);
                entity.Property(e => e.Notes).HasMaxLength(2000);
                entity.Property(e => e.RoomNumber).HasMaxLength(50);
                entity.Property(e => e.Floor).HasMaxLength(50);
                entity.Property(e => e.Building).HasMaxLength(100);
                entity.Property(e => e.ReferralNotes).HasMaxLength(1000);
                entity.Property(e => e.InsuranceProvider).HasMaxLength(200);
                entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(100);
                entity.Property(e => e.ConsultationFee).HasColumnType("decimal(18,2)");

                entity.HasMany(e => e.History)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Reminders)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.WaitlistEntry)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey<WaitlistEntry>(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // AppointmentHistory Configuration
            builder.Entity<AppointmentHistory>(entity =>
            {
                entity.ToTable("AppointmentHistories");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AppointmentId);
                entity.HasIndex(e => e.PerformedAt);

                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.OldValue).HasMaxLength(500);
                entity.Property(e => e.NewValue).HasMaxLength(500);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.PerformedByName).HasMaxLength(200);
            });

            // AppointmentReminder Configuration
            builder.Entity<AppointmentReminder>(entity =>
            {
                entity.ToTable("AppointmentReminders");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AppointmentId);
                entity.HasIndex(e => e.ScheduledFor);
                entity.HasIndex(e => new { e.IsSent, e.ScheduledFor });

                entity.Property(e => e.ErrorMessage).HasMaxLength(500);
            });

            // DoctorSchedule Configuration
            builder.Entity<DoctorSchedule>(entity =>
            {
                entity.ToTable("DoctorSchedules");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => new { e.DoctorId, e.DayOfWeek, e.IsActive });

                entity.Property(e => e.DoctorName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasMany(e => e.Exceptions)
                    .WithOne(e => e.Schedule)
                    .HasForeignKey(e => e.DoctorScheduleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // DoctorLeave Configuration
            builder.Entity<DoctorLeave>(entity =>
            {
                entity.ToTable("DoctorLeaves");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.DoctorName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Reason).HasMaxLength(500);
            });

            // ScheduleException Configuration
            builder.Entity<ScheduleException>(entity =>
            {
                entity.ToTable("ScheduleExceptions");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorScheduleId);
                entity.HasIndex(e => e.Date);

                entity.Property(e => e.Reason).HasMaxLength(500);
            });

            // WaitlistEntry Configuration
            builder.Entity<WaitlistEntry>(entity =>
            {
                entity.ToTable("WaitlistEntries");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.DoctorId, e.PreferredDate, e.Status });

                entity.Property(e => e.PatientName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PatientPhone).HasMaxLength(20);
                entity.Property(e => e.PatientEmail).HasMaxLength(256);
                entity.Property(e => e.DoctorName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            // TimeSlot Configuration
            builder.Entity<TimeSlot>(entity =>
            {
                entity.ToTable("TimeSlots");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.DoctorId, e.Date, e.StartTime }).IsUnique();
                entity.HasIndex(e => new { e.DoctorId, e.Date, e.IsAvailable });

                entity.Property(e => e.BlockReason).HasMaxLength(500);
            });

            // AppointmentConflict Configuration
            builder.Entity<AppointmentConflict>(entity =>
            {
                entity.ToTable("AppointmentConflicts");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AppointmentId);
                entity.HasIndex(e => new { e.IsResolved });

                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Resolution).HasMaxLength(1000);
            });
        }
    }

    // DbContext Factory for Migrations
    public class AppointmentDbContextFactory : IDesignTimeDbContextFactory<AppointmentDbContext>
    {
        public AppointmentDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HMS_MS_APPOINTMENT;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

            var optionsBuilder = new DbContextOptionsBuilder<AppointmentDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("HMS.Appointment.Infrastructure"));

            return new AppointmentDbContext(optionsBuilder.Options);
        }
    }
}