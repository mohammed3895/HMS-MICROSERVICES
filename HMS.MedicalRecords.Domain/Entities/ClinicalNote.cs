namespace HMS.MedicalRecords.Domain.Entities
{
    public class ClinicalNote
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string Subjective { get; set; } // Patient's complaints
        public string Objective { get; set; }  // Doctor's observations
        public string Assessment { get; set; } // Diagnosis
        public string Plan { get; set; }       // Treatment plan
        public string? AdditionalNotes { get; set; }
        public DateTime RecordedAt { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime? FinalizedAt { get; set; }
    }
}
