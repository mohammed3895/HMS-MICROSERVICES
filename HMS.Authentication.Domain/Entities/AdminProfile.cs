namespace HMS.Authentication.Domain.Entities
{
    public class AdminProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTime JoiningDate { get; set; }
        public List<string> Permissions { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
