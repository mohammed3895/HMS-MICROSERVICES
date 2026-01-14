namespace HMS.Authentication.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string? LicenseNumber { get; set; } // For medical staff
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Navigation
        public UserProfile Profile { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<LoginHistory> LoginHistories { get; set; }
    }
}
