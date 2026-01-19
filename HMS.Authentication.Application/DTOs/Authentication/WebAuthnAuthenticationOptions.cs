namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class WebAuthnAuthenticationOptions
    {
        public string Challenge { get; set; } = string.Empty;
        public string RpId { get; set; } = string.Empty;
        public int Timeout { get; set; }
        public List<string> AllowCredentials { get; set; } = new();
    }
}
