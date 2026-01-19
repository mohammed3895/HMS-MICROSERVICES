namespace APIGateway.Middlewares
{
    public class CircuitBreakerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CircuitBreakerMiddleware> _logger;
        private static readonly Dictionary<string, CircuitBreakerState> _circuitStates = new();
        private const int FAILURE_THRESHOLD = 5;
        private const int TIMEOUT_SECONDS = 30;

        public CircuitBreakerMiddleware(RequestDelegate next, ILogger<CircuitBreakerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var service = GetServiceFromPath(context.Request.Path);
            if (string.IsNullOrEmpty(service))
            {
                await _next(context);
                return;
            }

            if (!_circuitStates.TryGetValue(service, out var state))
            {
                state = new CircuitBreakerState();
                _circuitStates[service] = state;
            }

            if (state.IsOpen && DateTime.UtcNow < state.OpenUntil)
            {
                _logger.LogWarning("Circuit breaker open for service {Service}", service);
                context.Response.StatusCode = 503;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = $"Service {service} is temporarily unavailable",
                    retryAfter = (state.OpenUntil - DateTime.UtcNow).TotalSeconds
                });
                return;
            }

            try
            {
                await _next(context);

                // Reset on success
                if (context.Response.StatusCode < 500)
                {
                    state.Reset();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service {Service}", service);
                state.RecordFailure();

                if (state.FailureCount >= FAILURE_THRESHOLD)
                {
                    state.Open(TIMEOUT_SECONDS);
                    _logger.LogWarning("Circuit breaker opened for service {Service}", service);
                }

                throw;
            }
        }

        private string? GetServiceFromPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return segments.Length > 0 ? segments[0] : null;
        }

        private class CircuitBreakerState
        {
            public int FailureCount { get; private set; }
            public bool IsOpen { get; private set; }
            public DateTime OpenUntil { get; private set; }

            public void RecordFailure()
            {
                FailureCount++;
            }

            public void Reset()
            {
                FailureCount = 0;
                IsOpen = false;
            }

            public void Open(int timeoutSeconds)
            {
                IsOpen = true;
                OpenUntil = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            }
        }
    }
}
