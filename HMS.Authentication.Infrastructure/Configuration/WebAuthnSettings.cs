namespace HMS.Authentication.Infrastructure.Configuration
{
    public class WebAuthnSettings
    {
        public string RelyingPartyId { get; set; } = string.Empty;
        public string RelyingPartyName { get; set; } = "Hospital Management System";
        public string Origin { get; set; } = string.Empty;
        public int Timeout { get; set; } = 60000;
    }
}
