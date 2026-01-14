using HMS.Billing.Domain.Enums;

namespace HMS.Billing.Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public Guid PatientId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? AdmissionId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal LateFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? BillingAddress { get; set; }
        public string? BillingEmail { get; set; }
        public string? BillingPhone { get; set; }
        public string? Notes { get; set; }
        public string? TermsAndConditions { get; set; }
        public bool IsTaxable { get; set; }
        public string? TaxId { get; set; }
        public string? InsuranceClaimId { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public InsuranceClaim? InsuranceClaim { get; set; }
    }
}
