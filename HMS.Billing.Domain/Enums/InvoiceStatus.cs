namespace HMS.Billing.Domain.Enums
{
    public enum InvoiceStatus
    {
        Draft = 1,
        Generated = 2,
        Sent = 3,
        PartiallyPaid = 4,
        Paid = 5,
        Overdue = 6,
        Cancelled = 7,
        Refunded = 8,
        UnderReview = 9
    }
}
