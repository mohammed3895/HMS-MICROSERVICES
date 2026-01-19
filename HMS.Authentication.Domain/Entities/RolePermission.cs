namespace HMS.Authentication.Domain.Entities
{
    public class RolePermission
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Permission { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationRole Role { get; set; } = null!;
    }
}
