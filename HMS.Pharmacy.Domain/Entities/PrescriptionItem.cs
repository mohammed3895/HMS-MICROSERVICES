namespace HMS.Pharamcy.Dimain.Entities
{
    public class PrescriptionItem
    {
        public Guid Id { get; set; }
        public Guid PrescriptionId { get; set; }
        public Guid MedicineId { get; set; }
        public int Quantity { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public int Duration { get; set; } // in days
        public string? AdditionalInstructions { get; set; }

        public Prescription Prescription { get; set; }
        public Medicine Medicine { get; set; }
    }
}
