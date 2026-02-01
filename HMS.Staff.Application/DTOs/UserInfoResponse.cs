namespace HMS.Staff.Application.DTOs
{
    public class UserInfoResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
