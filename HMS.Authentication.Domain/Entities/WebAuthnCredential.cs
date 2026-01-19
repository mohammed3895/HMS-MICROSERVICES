namespace HMS.Authentication.Domain.Entities
{
    public class WebAuthnCredential
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CredentialId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public uint SignatureCounter { get; set; }
        public string? DeviceName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }

        // Navigation property
        public ApplicationUser User { get; set; } = null!;
    }
}
