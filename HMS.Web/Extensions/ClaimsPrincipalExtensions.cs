using System.Security.Claims;

namespace HMS.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim?.Value, out var userId) ? userId : Guid.Empty;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public static string GetFullName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static bool HasRole(this ClaimsPrincipal principal, string role)
        {
            return principal.FindFirst(ClaimTypes.Role)?.Value == role;
        }
    }
}
