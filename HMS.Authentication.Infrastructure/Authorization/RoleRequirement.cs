using Microsoft.AspNetCore.Authorization;

namespace HMS.Authentication.Infrastructure.Authorization
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string[] AllowedRoles { get; }

        public RoleRequirement(params string[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }
    }
}
