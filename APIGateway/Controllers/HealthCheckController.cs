using Microsoft.AspNetCore.Mvc;

namespace HMS.APIGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<HealthCheckController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Check health of all microservices
        /// </summary>
        [HttpGet("services")]
        public async Task<IActionResult> CheckAllServices()
        {
            var services = new[]
            {
                "Authentication",
                "Patient",
                "Doctor",
                "Staff",
                "Appointment",
                "MedicalRecords",
                "Billing",
                "Pharmacy",
                "Laboratory",
                "Notification"
            };

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var healthChecks = new Dictionary<string, object>();

            foreach (var service in services)
            {
                var serviceUrl = _configuration[$"ServiceEndpoints:{service}"];
                if (string.IsNullOrEmpty(serviceUrl))
                {
                    healthChecks[service] = new
                    {
                        status = "NotConfigured",
                        message = "Service endpoint not configured"
                    };
                    continue;
                }

                try
                {
                    var startTime = DateTime.UtcNow;
                    var response = await client.GetAsync($"{serviceUrl}/health");
                    var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                    healthChecks[service] = new
                    {
                        status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                        statusCode = (int)response.StatusCode,
                        responseTime = $"{responseTime}ms",
                        url = serviceUrl
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Health check failed for {Service}", service);
                    healthChecks[service] = new
                    {
                        status = "Unreachable",
                        error = ex.Message,
                        url = serviceUrl
                    };
                }
            }

            var overallStatus = healthChecks.All(h =>
                h.Value is not null &&
                h.Value.GetType().GetProperty("status")?.GetValue(h.Value)?.ToString() == "Healthy")
                ? "Healthy"
                : "Degraded";

            return Ok(new
            {
                gateway = "Healthy",
                overallStatus,
                services = healthChecks,
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Check specific service health
        /// </summary>
        [HttpGet("service/{serviceName}")]
        public async Task<IActionResult> CheckService(string serviceName)
        {
            var serviceUrl = _configuration[$"ServiceEndpoints:{serviceName}"];
            if (string.IsNullOrEmpty(serviceUrl))
            {
                return NotFound(new { error = $"Service '{serviceName}' not found or not configured" });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);

                var startTime = DateTime.UtcNow;
                var response = await client.GetAsync($"{serviceUrl}/health");
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                return Ok(new
                {
                    service = serviceName,
                    status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                    statusCode = (int)response.StatusCode,
                    responseTime = $"{responseTime}ms",
                    url = serviceUrl,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health check failed for {Service}", serviceName);
                return Ok(new
                {
                    service = serviceName,
                    status = "Unreachable",
                    error = ex.Message,
                    url = serviceUrl,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get gateway information
        /// </summary>
        [HttpGet("info")]
        public IActionResult GetGatewayInfo()
        {
            return Ok(new
            {
                name = "HMS API Gateway",
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                status = "Running",
                uptime = GetUptime(),
                timestamp = DateTime.UtcNow
            });
        }

        private string GetUptime()
        {
            var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
        }
    }
}