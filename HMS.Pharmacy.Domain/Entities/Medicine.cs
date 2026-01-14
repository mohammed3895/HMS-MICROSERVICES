namespace HMS.Pharamcy.Dimain.Entities
{
    public class Medicine
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string GenericName { get; set; }
        public string Brand { get; set; }
        public string DosageForm { get; set; }
        public string Strength { get; set; }
        public string? Manufacturer { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
