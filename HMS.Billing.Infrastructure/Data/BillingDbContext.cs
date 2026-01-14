using HMS.Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Billing.Infrastructure.Data
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentGatewayTransaction> PaymentGatewayTransactions { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
        public DbSet<BillableService> BillableServices { get; set; }
        public DbSet<TaxConfiguration> TaxConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Invoice Configuration
            builder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.InvoiceNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SubTotal)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TaxAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.LateFee)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TotalAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.PaidAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.BalanceAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.BillingAddress)
                    .HasMaxLength(500);

                entity.Property(e => e.BillingEmail)
                    .HasMaxLength(256);

                entity.Property(e => e.BillingPhone)
                    .HasMaxLength(20);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.TermsAndConditions)
                    .HasMaxLength(4000);

                entity.Property(e => e.TaxId)
                    .HasMaxLength(50);

                entity.Property(e => e.InsuranceClaimId)
                    .HasMaxLength(50);

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(200);

                // Indexes
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.AppointmentId);
                entity.HasIndex(e => e.AdmissionId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.InvoiceDate);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => new { e.PatientId, e.Status });

                // NOTE: PatientId references Patient service
                // AppointmentId references Appointment service
                // AdmissionId references Staff service (PatientAdmission)
                // No navigation properties across service boundaries

                // One-to-Many with InvoiceItems
                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One-to-Many with Payments
                entity.HasMany(e => e.Payments)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Restrict);

                // One-to-One with InsuranceClaim
                entity.HasOne(e => e.InsuranceClaim)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey<InsuranceClaim>(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // InvoiceItem Configuration
            builder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable("InvoiceItems");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ItemType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TotalPrice)
                    .HasPrecision(18, 2);

                // Indexes
                entity.HasIndex(e => e.InvoiceId);
                entity.HasIndex(e => e.ReferenceId);
                entity.HasIndex(e => e.ItemType);

                // NOTE: ReferenceId can reference different services
                // (Appointment, LabOrder, Pharmacy, etc.)
            });

            // Payment Configuration
            builder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.PaymentReference)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(200);

                entity.Property(e => e.BankReference)
                    .HasMaxLength(200);

                entity.Property(e => e.CardLastFourDigits)
                    .HasMaxLength(4);

                entity.Property(e => e.CardType)
                    .HasMaxLength(50);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                // Indexes
                entity.HasIndex(e => e.PaymentReference).IsUnique();
                entity.HasIndex(e => e.InvoiceId);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.PaymentDate);
                entity.HasIndex(e => e.TransactionId);

                // NOTE: PatientId references Patient service
            });

            // PaymentGatewayTransaction Configuration
            builder.Entity<PaymentGatewayTransaction>(entity =>
            {
                entity.ToTable("PaymentGatewayTransactions");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.GatewayTransactionId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.RequestData)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.ResponseData)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.ErrorCode)
                    .HasMaxLength(50);

                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(1000);

                // Indexes
                entity.HasIndex(e => e.PaymentId);
                entity.HasIndex(e => e.GatewayTransactionId);
                entity.HasIndex(e => e.Status);

                // One-to-One with Payment
                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Refund Configuration
            builder.Entity<Refund>(entity =>
            {
                entity.ToTable("Refunds");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.RefundNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RefundAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(200);

                // Indexes
                entity.HasIndex(e => e.RefundNumber).IsUnique();
                entity.HasIndex(e => e.PaymentId);
                entity.HasIndex(e => e.InvoiceId);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.RequestDate);

                // NOTE: PatientId references Patient service

                // Many-to-One with Payment
                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One with Invoice
                entity.HasOne(e => e.Invoice)
                    .WithMany()
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // InsuranceClaim Configuration
            builder.Entity<InsuranceClaim>(entity =>
            {
                entity.ToTable("InsuranceClaims");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ClaimNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.InsuranceProvider)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PolicyNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.GroupNumber)
                    .HasMaxLength(100);

                entity.Property(e => e.ClaimAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.ApprovedAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.PatientResponsibility)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DenialReason)
                    .HasMaxLength(1000);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.AttachmentPath)
                    .HasMaxLength(500);

                // Indexes
                entity.HasIndex(e => e.ClaimNumber).IsUnique();
                entity.HasIndex(e => e.InvoiceId);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ClaimDate);

                // NOTE: PatientId references Patient service
            });

            // BillableService Configuration
            builder.Entity<BillableService>(entity =>
            {
                entity.ToTable("BillableServices");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ServiceCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.StandardPrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.InsurancePrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.GovernmentPrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TaxRate)
                    .HasPrecision(5, 2);

                // Indexes
                entity.HasIndex(e => e.ServiceCode).IsUnique();
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);

                // One-to-Many with InvoiceItems
                entity.HasMany(e => e.InvoiceItems)
                    .WithMany()
                    .UsingEntity("BillableServiceInvoiceItems");
            });

            // TaxConfiguration Configuration
            builder.Entity<TaxConfiguration>(entity =>
            {
                entity.ToTable("TaxConfigurations");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.TaxCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TaxName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TaxRate)
                    .HasPrecision(5, 2);

                entity.Property(e => e.ApplicableServices)
                    .HasColumnType("nvarchar(max)");

                // Indexes
                entity.HasIndex(e => e.TaxCode).IsUnique();
                entity.HasIndex(e => e.IsActive);
            });

            // Add check constraints
            builder.Entity<Invoice>()
                .HasCheckConstraint("CK_Invoice_Amount", "[BalanceAmount] = [TotalAmount] - [PaidAmount]");

            builder.Entity<Payment>()
                .HasCheckConstraint("CK_Payment_Amount", "[Amount] > 0");

            builder.Entity<Refund>()
                .HasCheckConstraint("CK_Refund_Amount", "[RefundAmount] > 0");
        }
    }
}