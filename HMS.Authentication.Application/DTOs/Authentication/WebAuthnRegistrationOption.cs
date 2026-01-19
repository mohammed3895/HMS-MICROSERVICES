namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class WebAuthnRegistrationOptions
    {
        public string Challenge { get; set; } = string.Empty;
        public string RpId { get; set; } = string.Empty;
        public string RpName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public int Timeout { get; set; }
        public string Attestation { get; set; } = "none";
    }
}
