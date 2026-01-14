using HospitalManagement.Common.Enums;

namespace HMS.Patient.Domain.Entities
{
    public class Allergy
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string Allergen { get; set; } // Drug, Food, Environmental
        public string Reaction { get; set; }
        public SeverityLevel Severity { get; set; }
        public DateTime OnsetDate { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }

        public Patient Patient { get; set; }
    }
}
