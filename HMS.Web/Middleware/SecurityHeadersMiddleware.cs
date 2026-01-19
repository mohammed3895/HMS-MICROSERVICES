namespace HMS.Web.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers to all responses
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            // CSP for development (less restrictive)
            if (context.Response.ContentType?.Contains("text/html") == true)
            {
                context.Response.Headers.Append("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com; " +
                    "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self' https://cdnjs.cloudflare.com; " +
                    "connect-src 'self' https://localhost:7047;");
            }

            // Security logging
            if (context.Request.Path.StartsWithSegments("/Auth"))
            {
                _logger.LogInformation("Auth request: {Method} {Path} from {IP}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress);
            }

            await _next(context);
        }
    }
}