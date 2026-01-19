
using Microsoft.AspNetCore.Identity;

namespace HMS.Authentication.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    }
}
