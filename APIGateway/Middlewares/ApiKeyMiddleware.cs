namespace APIGateway.Middlewares
{
    /// <summary>
    /// API Key Middleware - Validates X-API-Key header for API Gateway access
    /// Allows public endpoints and authenticated requests to pass through
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyMiddleware> _logger;
        private const string API_KEY_HEADER = "X-API-Key";

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // ✅ PUBLIC ENDPOINTS - Skip all validation
            if (IsPublicEndpoint(path))
            {
                _logger.LogDebug("Public endpoint - skipping API Key validation: {Path}", path);
                await _next(context);
                return;
            }

            // ✅ AUTHENTICATED ENDPOINTS - Check for JWT Bearer token
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                _logger.LogDebug("JWT Bearer token found - allowing authenticated request");
                await _next(context);
                return;
            }

            // ✅ VALIDATE API KEY if provided
            var apiKey = GetApiKeyFromRequest(context);

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("No API Key or Bearer token provided for path: {Path} - Rejecting request", path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "API Key or Bearer token is required",
                    hint = "Include either 'X-API-Key' header or 'Authorization: Bearer {token}' header"
                });
                return;
            }

            // ✅ VALIDATE API KEY VALUE
            var configuredApiKey = _configuration["ApiKey"];
            if (!string.IsNullOrEmpty(configuredApiKey) && !configuredApiKey.Equals(apiKey))
            {
                _logger.LogWarning("Invalid API Key used for request to {Path}", path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
                return;
            }

            _logger.LogDebug("API Key validated for path: {Path}", path);
            await _next(context);
        }

        /// <summary>
        /// Check if path is public (doesn't require authentication)
        /// </summary>
        private bool IsPublicEndpoint(string path)
        {
            var publicPaths = new[]
            {
                "/health",
                "/swagger",
                
                // ✅ Ocelot upstream paths (without /api prefix)
                "/auth/login",
                "/auth/register",
                "/auth/confirm-email",
                "/auth/resend-otp",
                "/auth/verify-login-otp",
                "/auth/reset-password",
                "/auth/forgot-password",
                
                // ✅ 2FA PUBLIC ENDPOINTS - These need to be accessible without auth
                "/auth/2fa/verify-code",        // User verifying 2FA during login
                "/auth/2fa/use-recovery-code",  // User using recovery code during login
                
                // ✅ Direct API paths (with /api prefix - in case someone calls directly)
                "/api/auth/login",
                "/api/auth/register",
                "/api/auth/confirm-email",
                "/api/auth/resend-otp",
                "/api/auth/verify-login-otp",
                "/api/auth/reset-password",
                "/api/auth/forgot-password",
                "/api/auth/2fa/verify-code",
                "/api/auth/2fa/use-recovery-code",

                "/",
                "/info"
            };

            return publicPaths.Any(p => path == p || path.StartsWith(p + "/"));
        }

        /// <summary>
        /// Try to get API Key from request
        /// </summary>
        private string? GetApiKeyFromRequest(HttpContext context)
        {
            // Try X-API-Key header
            if (context.Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKeyHeader))
            {
                _logger.LogDebug("API Key found in {HeaderName} header", API_KEY_HEADER);
                return apiKeyHeader.ToString();
            }

            return null;
        }
    }
}