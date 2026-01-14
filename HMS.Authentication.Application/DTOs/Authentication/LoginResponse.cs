namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<string> Roles { get; set; }
    }
}
