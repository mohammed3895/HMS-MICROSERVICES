using Microsoft.AspNetCore.Identity;

namespace HMS.Authentication.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // Basic Information (Shared with Staff Service)
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Account Status
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Authentication
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public string? TwoFactorSecret { get; set; }
        public List<string> TwoFactorRecoveryCodes { get; set; } = new();
        public bool IsWebAuthnEnabled { get; set; }

        // Staff Integration
        public bool IsStaff { get; set; } // Indicates if user is a staff member
        public Guid? StaffServiceId { get; set; } // ID from Staff.Staff table (for reference)
        public string? StaffType { get; set; } // Doctor, Nurse, Pharmacist, etc.

        // Relationships
        public virtual UserProfile? Profile { get; set; }
        public virtual ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();
        public virtual ICollection<UserCredential> WebAuthnCredentials { get; set; } = new List<UserCredential>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    }
}
