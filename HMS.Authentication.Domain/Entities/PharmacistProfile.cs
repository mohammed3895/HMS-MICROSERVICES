namespace HMS.Authentication.Domain.Entities
{
    public class PharmacistProfile : BaseStaffProfile
    {
        public string PharmacyLicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string? PharmacyLocation { get; set; }
    }
}
