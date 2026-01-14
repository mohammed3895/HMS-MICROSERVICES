namespace HMS.Staff.Domain.Entities
{
    public class NurseNote
    {
        public Guid Id { get; set; }
        public Guid PatientAdmissionId { get; set; }
        public Guid NurseId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string? VitalSigns { get; set; } // JSON: BP, HR, Temp, SpO2, etc
        public DateTime RecordedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual PatientAdmission? PatientAdmission { get; set; }
        //public virtual Nurse? Nurse { get; set; }
    }
}
