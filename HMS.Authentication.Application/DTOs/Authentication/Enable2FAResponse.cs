namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class Enable2FAResponse
    {
        public string Secret { get; set; } = string.Empty;
        public string QrCodeUrl { get; set; } = string.Empty;
        public string ManualEntryKey { get; set; } = string.Empty;
    }
}
