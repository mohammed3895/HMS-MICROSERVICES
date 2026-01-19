namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class CreateStaffResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? EmployeeId { get; set; }
        public string TemporaryPassword { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
