using HMS.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Patient.Infrastructure.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options)
            : base(options)
        {
        }

        public DbSet<HMS.Patient.Domain.Entities.Patient> Patients { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Patient Configuration
            builder.Entity<HMS.Patient.Domain.Entities.Patient>(entity =>
            {
                entity.ToTable("Patients");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.InsuranceProvider)
                    .HasMaxLength(200);

                entity.Property(e => e.InsurancePolicyNumber)
                    .HasMaxLength(100);

                entity.Property(e => e.PrimaryCarePhysicianId)
                    .HasMaxLength(50);

                // Indexes
                entity.HasIndex(e => e.PatientId).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.RegistrationDate);

                // NOTE: UserId references Authentication.ApplicationUser
                // But we don't create navigation property to avoid circular dependency
                // This is just a foreign key reference

                // One-to-Many with MedicalHistory
                entity.HasMany(e => e.MedicalHistories)
                    .WithOne(e => e.Patient)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with Allergies
                entity.HasMany(e => e.Allergies)
                    .WithOne(e => e.Patient)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Note: Removed Appointments navigation property
                // AppointmentId will be stored in Appointment service database
            });

            // MedicalHistory Configuration
            builder.Entity<MedicalHistory>(entity =>
            {
                entity.ToTable("MedicalHistories");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.RecordType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.DiagnosisCode)
                    .HasMaxLength(50);

                entity.Property(e => e.ProcedureCode)
                    .HasMaxLength(50);

                entity.Property(e => e.AttachmentUrl)
                    .HasMaxLength(500);

                // Indexes
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.RecordDate);
                entity.HasIndex(e => e.RecordType);

                // NOTE: DoctorId and DepartmentId reference Staff service
                // Store only IDs, no navigation properties
            });

            // Allergy Configuration
            builder.Entity<Allergy>(entity =>
            {
                entity.ToTable("Allergies");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Allergen)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Reaction)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);

                // Indexes
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => new { e.PatientId, e.IsActive });
                entity.HasIndex(e => e.Severity);
            });

            // VitalSign Configuration
            builder.Entity<VitalSign>(entity =>
            {
                entity.ToTable("VitalSigns");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Temperature)
                    .HasPrecision(5, 2);

                entity.Property(e => e.OxygenSaturation)
                    .HasPrecision(5, 2);

                entity.Property(e => e.Height)
                    .HasPrecision(5, 2);

                entity.Property(e => e.Weight)
                    .HasPrecision(5, 2);

                entity.Property(e => e.Bmi)
                    .HasPrecision(5, 2);

                entity.Property(e => e.BloodPressure)
                    .HasMaxLength(20);

                // Indexes
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.RecordedAt);
                entity.HasIndex(e => new { e.PatientId, e.RecordedAt });

                // NOTE: RecordedBy references Staff service (StaffId)
                // Store only ID, no navigation property
            });
        }
    }
}