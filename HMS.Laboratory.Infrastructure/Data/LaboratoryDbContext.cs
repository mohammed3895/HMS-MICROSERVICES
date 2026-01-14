using HMS.Laboratory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Laboratory.Infrastructure.Data
{
    public class LaboratoryDbContext : DbContext
    {
        public LaboratoryDbContext(DbContextOptions<LaboratoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<LabTestParameter> LabTestParameters { get; set; }
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<LabResult> LabResults { get; set; }
        public DbSet<LabSample> LabSamples { get; set; }
        public DbSet<LabOrderComment> LabOrderComments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // LabTest Configuration
            builder.Entity<LabTest>(entity =>
            {
                entity.ToTable("LabTests");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.TestCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ShortName)
                    .HasMaxLength(100);

                entity.Property(e => e.SubCategory)
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(2000);

                entity.Property(e => e.SampleVolume)
                    .HasMaxLength(100);

                entity.Property(e => e.ContainerType)
                    .HasMaxLength(100);

                entity.Property(e => e.StorageInstructions)
                    .HasMaxLength(500);

                entity.Property(e => e.TransportInstructions)
                    .HasMaxLength(500);

                entity.Property(e => e.Methodology)
                    .HasMaxLength(500);

                entity.Property(e => e.Equipment)
                    .HasMaxLength(200);

                entity.Property(e => e.TurnaroundTime)
                    .HasMaxLength(100);

                entity.Property(e => e.ReferenceRangeMale)
                    .HasMaxLength(200);

                entity.Property(e => e.ReferenceRangeFemale)
                    .HasMaxLength(200);

                entity.Property(e => e.ReferenceRangeChild)
                    .HasMaxLength(200);

                entity.Property(e => e.ReferenceRangeNewborn)
                    .HasMaxLength(200);

                entity.Property(e => e.CriticalRange)
                    .HasMaxLength(200);

                entity.Property(e => e.Units)
                    .HasMaxLength(50);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.Property(e => e.InsurancePrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.GovernmentPrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.PreparationInstructions)
                    .HasMaxLength(1000);

                entity.Property(e => e.InterferingSubstances)
                    .HasMaxLength(1000);

                entity.Property(e => e.ClinicalSignificance)
                    .HasMaxLength(2000);

                entity.Property(e => e.CPTCode)
                    .HasMaxLength(50);

                entity.Property(e => e.LOINCCode)
                    .HasMaxLength(50);

                // Indexes
                entity.HasIndex(e => e.TestCode).IsUnique();
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.SampleType);

                // One-to-Many with LabTestParameters
                entity.HasMany(e => e.Parameters)
                    .WithOne(e => e.Test)
                    .HasForeignKey(e => e.TestId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with LabOrders
                entity.HasMany(e => e.LabOrders)
                    .WithOne(e => e.Test)
                    .HasForeignKey(e => e.TestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // LabTestParameter Configuration
            builder.Entity<LabTestParameter>(entity =>
            {
                entity.ToTable("LabTestParameters");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ParameterCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ParameterName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ReferenceRange)
                    .HasMaxLength(200);

                entity.Property(e => e.Units)
                    .HasMaxLength(50);

                entity.Property(e => e.Method)
                    .HasMaxLength(200);

                entity.Property(e => e.CriticalRange)
                    .HasMaxLength(200);

                // Indexes
                entity.HasIndex(e => e.TestId);
                entity.HasIndex(e => new { e.TestId, e.ParameterCode }).IsUnique();
                entity.HasIndex(e => e.SortOrder);
            });

            // LabOrder Configuration
            builder.Entity<LabOrder>(entity =>
            {
                entity.ToTable("LabOrders");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ClinicalNotes)
                    .HasMaxLength(2000);

                entity.Property(e => e.DiagnosisCode)
                    .HasMaxLength(50);

                entity.Property(e => e.SpecimenType)
                    .HasMaxLength(100);

                entity.Property(e => e.CollectionSite)
                    .HasMaxLength(200);

                entity.Property(e => e.CollectionNotes)
                    .HasMaxLength(1000);

                entity.Property(e => e.SampleVolume)
                    .HasPrecision(10, 2);

                entity.Property(e => e.SampleUnit)
                    .HasMaxLength(50);

                entity.Property(e => e.InstrumentUsed)
                    .HasMaxLength(200);

                entity.Property(e => e.ReagentLotNumber)
                    .HasMaxLength(100);

                entity.Property(e => e.VerificationNotes)
                    .HasMaxLength(1000);

                entity.Property(e => e.ReportDeliveryMethod)
                    .HasMaxLength(50);

                entity.Property(e => e.CancellationReason)
                    .HasMaxLength(1000);

                // Indexes
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.TestId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.OrderDate);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => new { e.PatientId, e.OrderDate });
                entity.HasIndex(e => new { e.DoctorId, e.OrderDate });

                // NOTE: PatientId references Patient service
                // DoctorId references Staff service
                // InvoiceId references Billing service
                // CollectedBy, PerformedBy, VerifiedBy, CreatedBy reference Staff service
                // No navigation properties across service boundaries

                // One-to-One with LabResult
                entity.HasOne(e => e.Result)
                    .WithOne(e => e.Order)
                    .HasForeignKey<LabResult>(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with LabSamples
                entity.HasMany(e => e.Samples)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with LabOrderComments
                entity.HasMany(e => e.Comments)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // LabResult Configuration
            builder.Entity<LabResult>(entity =>
            {
                entity.ToTable("LabResults");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ResultData)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Interpretation)
                    .HasMaxLength(2000);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.AbnormalFlags)
                    .HasMaxLength(50);

                entity.Property(e => e.CriticalRange)
                    .HasMaxLength(200);

                entity.Property(e => e.ReferenceRange)
                    .HasMaxLength(200);

                entity.Property(e => e.Units)
                    .HasMaxLength(50);

                entity.Property(e => e.Method)
                    .HasMaxLength(200);

                entity.Property(e => e.Equipment)
                    .HasMaxLength(200);

                entity.Property(e => e.ReportFormat)
                    .HasMaxLength(50);

                entity.Property(e => e.ReportPath)
                    .HasMaxLength(500);

                // Indexes
                entity.HasIndex(e => e.OrderId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ResultDate);
                entity.HasIndex(e => e.IsAbnormal);
                entity.HasIndex(e => e.IsCritical);

                // NOTE: VerifiedBy references Staff service
            });

            // LabSample Configuration
            builder.Entity<LabSample>(entity =>
            {
                entity.ToTable("LabSamples");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.SampleId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ContainerType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Color)
                    .HasMaxLength(50);

                entity.Property(e => e.Volume)
                    .HasPrecision(10, 2);

                entity.Property(e => e.Unit)
                    .HasMaxLength(50);

                entity.Property(e => e.Appearance)
                    .HasMaxLength(200);

                entity.Property(e => e.StorageLocation)
                    .HasMaxLength(200);

                entity.Property(e => e.StorageTemperature)
                    .HasMaxLength(100);

                entity.Property(e => e.SampleCondition)
                    .HasMaxLength(200);

                entity.Property(e => e.RejectionReason)
                    .HasMaxLength(1000);

                // Indexes
                entity.HasIndex(e => e.SampleId).IsUnique();
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.IsRejected);
                entity.HasIndex(e => e.CollectionDate);

                // Many-to-Many with LabTests
                entity.HasMany(e => e.Tests)
                    .WithMany()
                    .UsingEntity("LabSampleTests");
            });

            // LabOrderComment Configuration
            builder.Entity<LabOrderComment>(entity =>
            {
                entity.ToTable("LabOrderComments");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserRole)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasMaxLength(2000);

                // Indexes
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CommentDate);
                entity.HasIndex(e => e.IsInternal);

                // NOTE: UserId references Authentication service
            });
        }
    }
}