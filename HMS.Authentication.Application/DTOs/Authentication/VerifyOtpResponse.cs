namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class VerifyOtpResponse
    {
        public bool IsVerified { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; }
    }
}
