using System.Diagnostics;

namespace APIGateway.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            context.Items["RequestId"] = requestId;
            context.Response.Headers.Add("X-Request-Id", requestId);

            try
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation(
                    "Request {RequestId} {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Request {RequestId} {Method} {Path} failed after {ElapsedMs}ms",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
