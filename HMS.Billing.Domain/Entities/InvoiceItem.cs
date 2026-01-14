namespace HMS.Billing.Domain.Entities
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string ItemType { get; set; } // Consultation, LabTest, Medicine, Procedure
        public Guid? ReferenceId { get; set; } // Link to appointment, lab order, etc.
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public Invoice Invoice { get; set; }
    }
}
