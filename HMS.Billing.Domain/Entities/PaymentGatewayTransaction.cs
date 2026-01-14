using HMS.Billing.Domain.Enums;

namespace HMS.Billing.Domain.Entities
{
    public class PaymentGatewayTransaction
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public string GatewayTransactionId { get; set; }
        public PaymentGateway Gateway { get; set; }
        public string RequestData { get; set; } // JSON
        public string ResponseData { get; set; } // JSON
        public Enums.TransactionStatus Status { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Payment Payment { get; set; }
    }
}
