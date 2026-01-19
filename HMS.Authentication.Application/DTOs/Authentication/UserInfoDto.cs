namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class UserInfoDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string? ProfilePictureUrl { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsWebAuthnEnabled { get; set; }
    }
}
