namespace HMS.Staff.Application.DTOs
{
    public class StaffSummaryDto
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string StaffType { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
    }
}
