namespace HMS.Staff.Application.DTOs
{

    public class CreateStaffAuthRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Doctor, Nurse, etc.
        public Guid StaffServiceId { get; set; } // ID from Staff.Staff table
    }
}
