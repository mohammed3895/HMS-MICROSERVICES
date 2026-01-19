using APIGateway.Middlewares;

namespace APIGateway.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }

        public static IApplicationBuilder UseApiKey(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }

        public static IApplicationBuilder UseCircuitBreaker(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CircuitBreakerMiddleware>();
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
