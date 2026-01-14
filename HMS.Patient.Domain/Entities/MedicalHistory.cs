namespace HMS.Patient.Domain.Entities
{
    public class MedicalHistory
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; } // Diagnosis, Procedure, Immunization, etc.
        public string Description { get; set; }
        public string? DiagnosisCode { get; set; } // ICD-10/ICD-11
        public string? ProcedureCode { get; set; } // CPT
        public string? Notes { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public Patient Patient { get; set; }
    }
}
