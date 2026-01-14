using HMS.Billing.Domain.Enums;

namespace HMS.Billing.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string PaymentReference { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid? PatientId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string? BankReference { get; set; }
        public string? CardLastFourDigits { get; set; }
        public string? CardType { get; set; }
        public string? Notes { get; set; }
        public Guid? ReceivedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Invoice Invoice { get; set; }
    }
}
