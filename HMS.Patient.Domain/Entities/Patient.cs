using HMS.Patient.Domain.Enums;

namespace HMS.Patient.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PatientId { get; set; } // Hospital-specific ID (e.g., HOSP-001)
        public DateTime RegistrationDate { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public DateTime? InsuranceExpiry { get; set; }
        public string? PrimaryCarePhysicianId { get; set; }
        public PatientStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public ICollection<MedicalHistory> MedicalHistories { get; set; }
        public ICollection<Allergy> Allergies { get; set; }
    }
}
