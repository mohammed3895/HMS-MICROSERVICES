namespace HMS.Authentication.Domain.Entities
{
    public class UserCredential
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public byte[] CredentialId { get; set; } = Array.Empty<byte>();
        public byte[] PublicKey { get; set; } = Array.Empty<byte>();
        public byte[] UserHandle { get; set; } = Array.Empty<byte>();
        public uint SignatureCounter { get; set; }
        public string CredType { get; set; } = string.Empty;
        public Guid AaGuid { get; set; }
        public string DeviceName { get; set; } = "Unnamed Device";
        public bool IsBackupEligible { get; set; }
        public bool IsBackedUp { get; set; }
        public string? AttestationFormat { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
