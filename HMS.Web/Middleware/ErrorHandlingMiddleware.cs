namespace HMS.Web.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error");
                await HandleHttpExceptionAsync(context, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access");
                context.Response.Redirect("/auth/login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "An internal server error occurred",
                message = exception.Message,
                timestamp = DateTime.UtcNow
            };

            return context.Response.WriteAsJsonAsync(response);
        }

        private static Task HandleHttpExceptionAsync(HttpContext context, HttpRequestException exception)
        {
            context.Response.StatusCode = (int?)exception.StatusCode ?? StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Service communication error",
                message = "Unable to communicate with the backend service. Please try again later.",
                timestamp = DateTime.UtcNow
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
