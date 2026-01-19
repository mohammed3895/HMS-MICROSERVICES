namespace APIGateway.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CORRELATION_ID_HEADER = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetOrCreateCorrelationId(context);
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers.Add(CORRELATION_ID_HEADER, correlationId);

            await _next(context);
        }

        private string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CORRELATION_ID_HEADER, out var correlationId))
            {
                return correlationId.ToString();
            }

            return Guid.NewGuid().ToString();
        }
    }
}
