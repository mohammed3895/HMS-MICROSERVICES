namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool EmailConfirmationRequired { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
