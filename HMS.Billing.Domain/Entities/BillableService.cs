namespace HMS.Billing.Domain.Entities
{
    public class BillableService
    {
        public Guid Id { get; set; }
        public string ServiceCode { get; set; } // CPT/HCPCS/ICD codes
        public string ServiceName { get; set; }
        public string Category { get; set; } // Consultation, Lab, Pharmacy, Procedure, etc.
        public string Description { get; set; }
        public decimal StandardPrice { get; set; }
        public decimal InsurancePrice { get; set; }
        public decimal GovernmentPrice { get; set; }
        public decimal TaxRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
