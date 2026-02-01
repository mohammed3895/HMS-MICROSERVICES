namespace HMS.Staff.Application.DTOs
{
    public class StaffWithUserInfoDto
    {
        public Guid Id { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }

        // From ApplicationUser
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NationalId { get; set; } = string.Empty;

        // Staff-Specific
        public string StaffType { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string Position { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public string EmploymentStatus { get; set; } = string.Empty;
    }
}
