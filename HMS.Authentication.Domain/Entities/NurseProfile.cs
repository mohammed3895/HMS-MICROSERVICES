namespace HMS.Authentication.Domain.Entities
{
    public class NurseProfile : BaseStaffProfile
    {
        public string NursingLicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string Shift { get; set; } = string.Empty; // Morning, Evening, Night
        public string? Ward { get; set; }
    }
}
