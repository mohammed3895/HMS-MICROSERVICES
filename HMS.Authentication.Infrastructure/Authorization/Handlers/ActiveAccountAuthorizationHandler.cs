using HMS.Authentication.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HMS.Authentication.Infrastructure.Authorization.Handlers
{
    public class ActiveAccountAuthorizationHandler : AuthorizationHandler<ActiveAccountRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ActiveAccountAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ActiveAccountRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var userId = context.User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user?.IsActive == true)
            {
                context.Succeed(requirement);
            }
        }
    }
}
