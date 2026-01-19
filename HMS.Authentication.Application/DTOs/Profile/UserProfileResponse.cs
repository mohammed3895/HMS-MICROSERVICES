namespace HMS.Authentication.Application.DTOs.Profile
{
    public class UserProfileResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new();

        // General Profile
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? BloodGroup { get; set; }
        public string? Allergies { get; set; }

        // Role-Specific Profile
        public DoctorProfileDto? DoctorProfile { get; set; }
        public NurseProfileDto? NurseProfile { get; set; }
        public PharmacistProfileDto? PharmacistProfile { get; set; }
        public LabTechnicianProfileDto? LabTechnicianProfile { get; set; }
        public ReceptionistProfileDto? ReceptionistProfile { get; set; }
        public AdminProfileDto? AdminProfile { get; set; }
    }
}
