using HMS.Authentication.Domain.DTOs.Settings;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Authentication.Domain.Entities
{
    public class UserSettings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public NotificationSettingsDto NotificationSettings { get; set; } = new();

        [Column(TypeName = "nvarchar(max)")]
        public PrivacySettingsDto PrivacySettings { get; set; } = new();

        [Column(TypeName = "nvarchar(max)")]
        public PreferencesDto Preferences { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }
    }
}
