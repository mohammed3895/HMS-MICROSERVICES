
using Microsoft.AspNetCore.Identity;

namespace HMS.Authentication.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
        public bool IsSystemRole { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
