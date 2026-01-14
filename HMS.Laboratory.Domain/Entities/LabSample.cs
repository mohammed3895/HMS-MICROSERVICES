namespace HMS.Laboratory.Domain.Entities
{
    public class LabSample
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string SampleId { get; set; } // Laboratory sample ID
        public string ContainerType { get; set; } // Tube, vial, swab, etc.
        public string? Color { get; set; } // Tube color
        public decimal? Volume { get; set; }
        public string? Unit { get; set; }
        public string? Appearance { get; set; } // Clear, cloudy, hemolyzed, etc.
        public DateTime? CollectionDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? StorageLocation { get; set; }
        public string? StorageTemperature { get; set; } // Room temp, refrigerated, frozen
        public string? SampleCondition { get; set; } // Good, clotted, insufficient, etc.
        public string? RejectionReason { get; set; }
        public bool IsRejected { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public LabOrder Order { get; set; }
        public ICollection<LabTest> Tests { get; set; } = new List<LabTest>();
    }
}
