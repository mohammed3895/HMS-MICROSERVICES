namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class WebAuthnCredentialInfo
    {
        public Guid Id { get; set; }
        public string DeviceName { get; set; } = "Unknown Device";
        public DateTime CreatedAt { get; set; }
        public uint SignatureCounter { get; set; }
        public string CredType { get; set; } = string.Empty;
        public bool IsBackupEligible { get; set; }
        public bool IsBackedUp { get; set; }
        public DateTime LastUsedAt { get; set; }
    }
}
