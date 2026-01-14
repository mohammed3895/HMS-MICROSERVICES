namespace HMS.Billing.Domain.Enums
{
    public enum TransactionStatus
    {
        Initiated = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        Cancelled = 5,
        Refunded = 6,
        PartiallyRefunded = 7,
        Chargeback = 8
    }
}
