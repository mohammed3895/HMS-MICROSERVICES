using Polly;
using Polly.Extensions.Http;

namespace HMS.Web.Policies
{
    public static class HttpClientPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger? logger = null)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        var reason = outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Unknown";
                        logger?.LogWarning(
                            "HTTP Retry {RetryAttempt} after {TotalSeconds:F2}s. Reason: {Reason}",
                            retryAttempt, timespan.TotalSeconds, reason);
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger? logger = null)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, duration) =>
                    {
                        var reason = outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Unknown";
                        logger?.LogError(
                            "Circuit breaker OPENED for {Duration:F0}s. Reason: {Reason}",
                            duration.TotalSeconds, reason);
                    },
                    onReset: () =>
                    {
                        logger?.LogInformation("Circuit breaker RESET - requests flowing again");
                    },
                    onHalfOpen: () =>
                    {
                        logger?.LogInformation("Circuit breaker HALF-OPEN - testing service");
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(ILogger? logger = null)
        {
            var retry = GetRetryPolicy(logger);
            var circuitBreaker = GetCircuitBreakerPolicy(logger);

            return Policy.WrapAsync(circuitBreaker, retry);
        }
    }
}