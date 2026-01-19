namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class Verify2FASetupResponse
    {
        public bool IsVerified { get; set; }
        public List<string> RecoveryCodes { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
