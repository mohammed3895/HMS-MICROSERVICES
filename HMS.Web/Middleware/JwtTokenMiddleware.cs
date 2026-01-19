using HMS.Web.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace HMS.Web.Middleware
{
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtTokenMiddleware> _logger;

        public JwtTokenMiddleware(RequestDelegate next, ILogger<JwtTokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            // Skip for auth-related endpoints to avoid loops
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.Contains("/auth/") || path.Contains("/home/"))
            {
                await _next(context);
                return;
            }

            // Only process for authenticated users
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var token = await authService.GetAccessTokenAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    // Check if token is expired
                    if (IsTokenExpired(token))
                    {
                        _logger.LogInformation("Token expired, attempting refresh");

                        // Try to refresh the token
                        var refreshed = await authService.RefreshTokenAsync();

                        if (!refreshed)
                        {
                            _logger.LogWarning("Token refresh failed, logging out user");
                            await authService.LogoutAsync();

                            // Only redirect if this is a page request, not an AJAX/API call
                            if (!IsAjaxRequest(context))
                            {
                                context.Response.Redirect("/auth/login");
                                return;
                            }
                        }
                        else
                        {
                            // Get the new token after refresh
                            token = await authService.GetAccessTokenAsync();
                        }
                    }

                    // Add token to request header for API calls
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Request.Headers.Authorization = $"Bearer {token}";
                    }
                }
            }

            await _next(context);
        }

        private static bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return true;

                // Add 5 minute buffer before actual expiration
                return jwtToken.ValidTo.AddMinutes(-5) < DateTime.UtcNow;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private static bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   context.Request.Headers["Accept"].ToString().Contains("application/json");
        }
    }
}