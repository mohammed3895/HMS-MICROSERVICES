namespace HMS.Authentication.Application.DTOs.Profile
{
    public class DoctorProfileDto
    {
        public Guid Id { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string MedicalLicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string? EmployeeId { get; set; }
        public decimal? Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? Qualification { get; set; }
        public int? YearsOfExperience { get; set; }
        public List<string> Certifications { get; set; } = new();
        public string? ConsultationFee { get; set; }
        public int? MaxPatientsPerDay { get; set; }
        public string? Biography { get; set; }
        public bool IsAvailableForConsultation { get; set; }
    }
}
