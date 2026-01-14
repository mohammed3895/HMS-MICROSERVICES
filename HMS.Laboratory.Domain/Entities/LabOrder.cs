namespace HMS.Laboratory.Domain.Entities
{
    public class LabOrder
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid TestId { get; set; }

        // Order Details
        public DateTime OrderDate { get; set; }
        public OrderPriority Priority { get; set; }
        public string? ClinicalNotes { get; set; }
        public string? DiagnosisCode { get; set; } // ICD-10/ICD-11
        public string? SpecimenType { get; set; }
        public string? CollectionSite { get; set; }

        // Status Tracking
        public OrderStatus Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public TimeSpan? ScheduledTime { get; set; }

        // Sample Collection
        public Guid? CollectedBy { get; set; }
        public DateTime? CollectionDate { get; set; }
        public TimeSpan? CollectionTime { get; set; }
        public string? CollectionNotes { get; set; }
        public decimal? SampleVolume { get; set; }
        public string? SampleUnit { get; set; }

        // Testing
        public Guid? PerformedBy { get; set; }
        public DateTime? PerformedDate { get; set; }
        public TimeSpan? PerformedTime { get; set; }
        public string? InstrumentUsed { get; set; }
        public string? ReagentLotNumber { get; set; }

        // Verification
        public Guid? VerifiedBy { get; set; }
        public DateTime? VerificationDate { get; set; }
        public TimeSpan? VerificationTime { get; set; }
        public string? VerificationNotes { get; set; }

        // Reporting
        public DateTime? ReportDate { get; set; }
        public string? ReportDeliveryMethod { get; set; } // Email, Print, Portal
        public bool IsReportDelivered { get; set; }
        public DateTime? ReportDeliveryDate { get; set; }

        // Financial
        public bool IsBilled { get; set; }
        public Guid? InvoiceId { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string? CancellationReason { get; set; }
        public Guid? CancelledBy { get; set; }
        public DateTime? CancellationDate { get; set; }

        public LabTest Test { get; set; }
        public LabResult? Result { get; set; }
        public ICollection<LabSample> Samples { get; set; } = new List<LabSample>();
        public ICollection<LabOrderComment> Comments { get; set; } = new List<LabOrderComment>();
    }
}
