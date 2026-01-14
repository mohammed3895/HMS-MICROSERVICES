using HMS.Appointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Appointment.Infrastructure.Data
{
    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
            : base(options)
        {
        }

        public DbSet<HMS.Appointment.Domain.Entities.Appointment> Appointments { get; set; }
        public DbSet<AppointmentSlot> AppointmentSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Appointment Configuration
            builder.Entity<HMS.Appointment.Domain.Entities.Appointment>(entity =>
            {
                entity.ToTable("Appointments");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.AppointmentNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Reason)
                    .HasMaxLength(500);

                entity.Property(e => e.Symptoms)
                    .HasMaxLength(1000);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                // Indexes
                entity.HasIndex(e => e.AppointmentNumber).IsUnique();
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.AppointmentDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.DoctorId, e.AppointmentDate, e.Status });
                entity.HasIndex(e => new { e.PatientId, e.AppointmentDate });

                // One-to-Many with AppointmentSlots (within same service)
                entity.HasMany(e => e.Slots)
                    .WithOne(e => e.Appointment)
                    .HasForeignKey(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // AppointmentSlot Configuration
            builder.Entity<AppointmentSlot>(entity =>
            {
                entity.ToTable("AppointmentSlots");

                entity.HasKey(e => e.Id);

                // Indexes
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.SlotDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsAvailable);
                entity.HasIndex(e => new { e.DoctorId, e.SlotDate, e.IsAvailable });
                entity.HasIndex(e => new { e.DoctorId, e.SlotDate, e.StartTime, e.EndTime })
                    .IsUnique()
                    .HasFilter("[IsAvailable] = 1"); // Unique constraint only for available slots
            });

            // Seed some data or add constraints
            builder.Entity<HMS.Appointment.Domain.Entities.Appointment>()
                .HasCheckConstraint("CK_Appointment_Time", "[EndTime] > [StartTime]");

            builder.Entity<AppointmentSlot>()
                .HasCheckConstraint("CK_AppointmentSlot_Time", "[EndTime] > [StartTime]");
        }
    }
}