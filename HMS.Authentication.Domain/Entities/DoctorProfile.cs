namespace HMS.Authentication.Domain.Entities
{
    public class DoctorProfile : BaseStaffProfile
    {
        public string MedicalLicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public List<string> Certifications { get; set; } = new();
        public string? ConsultationFee { get; set; }
        public int MaxPatientsPerDay { get; set; }
        public string? Biography { get; set; }
    }
}
