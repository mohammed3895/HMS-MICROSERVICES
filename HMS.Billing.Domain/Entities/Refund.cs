using HMS.Billing.Domain.Enums;

namespace HMS.Billing.Domain.Entities
{
    public class Refund
    {
        public Guid Id { get; set; }
        public string RefundNumber { get; set; }
        public Guid PaymentId { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid PatientId { get; set; }
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; }
        public RefundStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public Guid? ProcessedBy { get; set; }
        public string? Notes { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Payment Payment { get; set; }
        public Invoice Invoice { get; set; }
    }
}
