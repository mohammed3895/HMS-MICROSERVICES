namespace HMS.Billing.Domain.Entities
{
    public class TaxConfiguration
    {
        public Guid Id { get; set; }
        public string TaxCode { get; set; }
        public string TaxName { get; set; }
        public decimal TaxRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? ApplicableServices { get; set; } // JSON array of service types
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
