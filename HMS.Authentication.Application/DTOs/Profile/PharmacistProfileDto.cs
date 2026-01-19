namespace HMS.Authentication.Application.DTOs.Profile
{
    public class PharmacistProfileDto
    {
        public Guid Id { get; set; }
        public string? Department { get; set; }
        public string PharmacyLicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string? EmployeeId { get; set; }
        public decimal? Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? PharmacyLocation { get; set; }
    }
}
