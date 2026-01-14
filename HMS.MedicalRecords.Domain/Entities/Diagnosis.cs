using HospitalManagement.MedicalRecords.API.Enums;

namespace HMS.MedicalRecords.Domain.Entities
{
{
    public class Diagnosis
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string DiagnosisCode { get; set; } // ICD-10/ICD-11
        public string Description { get; set; }
        public DiagnosisType Type { get; set; } // Primary, Secondary, etc.
        public DateTime DiagnosedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? Notes { get; set; }

        public HospitalManagement.Patient.API.Entities.Patient Patient { get; set; }
        public HospitalManagement.Staff.API.Entities.Staff Doctor { get; set; }
    }
}
