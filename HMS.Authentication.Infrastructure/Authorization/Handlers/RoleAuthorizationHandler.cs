using Microsoft.AspNetCore.Authorization;

namespace HMS.Authentication.Infrastructure.Authorization.Handlers
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return Task.CompletedTask;
            }

            var hasRole = requirement.AllowedRoles.Any(role =>
                context.User.IsInRole(role));

            if (hasRole)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
