using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace APIGateway.Services
{
    public interface ISwaggerAggregatorService
    {
        Task<string> GetAggregatedSwaggerJsonAsync();
    }

    public class SwaggerAggregatorService : ISwaggerAggregatorService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SwaggerAggregatorService> _logger;
        private readonly IWebHostEnvironment _environment;

        public SwaggerAggregatorService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache cache,
            ILogger<SwaggerAggregatorService> logger,
            IWebHostEnvironment environment)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
            _environment = environment;
        }

        public async Task<string> GetAggregatedSwaggerJsonAsync()
        {
            const string cacheKey = "aggregated_swagger";

            if (_cache.TryGetValue(cacheKey, out string cachedSwagger))
            {
                _logger.LogDebug("Returning cached swagger documentation");
                return cachedSwagger;
            }

            var aggregated = await AggregateSwaggerFromServices();
            _cache.Set(cacheKey, aggregated, TimeSpan.FromMinutes(5));

            return aggregated;
        }

        private async Task<string> AggregateSwaggerFromServices()
        {
            var baseSwagger = new
            {
                openapi = "3.0.1",
                info = new
                {
                    title = "HMS API Gateway - All Services",
                    version = "1.0.0",
                    description = "Combined API documentation for all microservices"
                },
                servers = new[]
                {
                    new { url = "https://localhost:7047", description = "Gateway Server" }
                },
                paths = new Dictionary<string, object>(),
                components = new
                {
                    schemas = new Dictionary<string, object>(),
                    securitySchemes = new Dictionary<string, object>
                    {
                        ["Bearer"] = new
                        {
                            type = "apiKey",
                            name = "Authorization",
                            @in = "header",
                            description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
                        }
                    }
                },
                security = new[] { new { Bearer = Array.Empty<string>() } }
            };

            var services = new Dictionary<string, string>
            {
                ["Authentication"] = _configuration["ServiceEndpoints:Authentication"] + "/swagger/v1/swagger.json",
                ["Patient"] = _configuration["ServiceEndpoints:Patient"] + "/swagger/v1/swagger.json",
                ["Doctor"] = _configuration["ServiceEndpoints:Doctor"] + "/swagger/v1/swagger.json",
                ["Staff"] = _configuration["ServiceEndpoints:Staff"] + "/swagger/v1/swagger.json",
                ["Appointment"] = _configuration["ServiceEndpoints:Appointment"] + "/swagger/v1/swagger.json",
                ["MedicalRecords"] = _configuration["ServiceEndpoints:MedicalRecords"] + "/swagger/v1/swagger.json",
                ["Billing"] = _configuration["ServiceEndpoints:Billing"] + "/swagger/v1/swagger.json",
                ["Pharmacy"] = _configuration["ServiceEndpoints:Pharmacy"] + "/swagger/v1/swagger.json",
                ["Laboratory"] = _configuration["ServiceEndpoints:Laboratory"] + "/swagger/v1/swagger.json"
            };

            // Create HttpClient with SSL bypass for development
            HttpClient client;
            if (_environment.IsDevelopment())
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                client = new HttpClient(handler);
            }
            else
            {
                client = _httpClientFactory.CreateClient();
            }

            client.Timeout = TimeSpan.FromSeconds(30);

            var successfulServices = 0;
            var failedServices = 0;

            foreach (var service in services)
            {
                try
                {
                    _logger.LogInformation("Attempting to fetch Swagger from {Service} at {Url}", service.Key, service.Value);

                    var response = await client.GetAsync(service.Value);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Service {Service} returned status code {StatusCode}",
                            service.Key, response.StatusCode);
                        failedServices++;
                        continue;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var serviceDoc = JsonSerializer.Deserialize<JsonElement>(content);

                    if (serviceDoc.TryGetProperty("paths", out var paths))
                    {
                        var pathCount = 0;
                        foreach (var path in paths.EnumerateObject())
                        {
                            var gatewayPath = ConvertToGatewayPath(path.Name, service.Key);

                            // Avoid duplicate paths
                            if (!baseSwagger.paths.ContainsKey(gatewayPath))
                            {
                                baseSwagger.paths[gatewayPath] = path.Value;
                                pathCount++;
                            }
                            else
                            {
                                _logger.LogWarning("Duplicate path detected: {Path} from {Service}", gatewayPath, service.Key);
                            }
                        }

                        _logger.LogInformation("Successfully added {Count} paths from {Service}", pathCount, service.Key);
                        successfulServices++;
                    }
                    else
                    {
                        _logger.LogWarning("No paths found in Swagger from {Service}", service.Key);
                        failedServices++;
                    }

                    // Also aggregate schemas if available
                    if (serviceDoc.TryGetProperty("components", out var components))
                    {
                        if (components.TryGetProperty("schemas", out var schemas))
                        {
                            foreach (var schema in schemas.EnumerateObject())
                            {
                                var schemaKey = $"{service.Key}{schema.Name}";
                                if (!baseSwagger.components.schemas.ContainsKey(schemaKey))
                                {
                                    baseSwagger.components.schemas[schemaKey] = schema.Value;
                                }
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Failed to connect to {Service} - Service may not be running at {Url}",
                        service.Key, service.Value);
                    failedServices++;
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "Request to {Service} timed out - Service may be slow or not responding at {Url}",
                        service.Key, service.Value);
                    failedServices++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error processing Swagger from {Service}", service.Key);
                    failedServices++;
                }
            }

            client.Dispose();

            // Add gateway's own endpoints
            AddGatewayEndpoints(baseSwagger.paths);

            _logger.LogInformation(
                "Swagger aggregation complete: {Total} paths from {Success} services ({Failed} failed)",
                baseSwagger.paths.Count,
                successfulServices,
                failedServices);

            return JsonSerializer.Serialize(baseSwagger, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        private string ConvertToGatewayPath(string originalPath, string serviceName)
        {
            return serviceName.ToLower() switch
            {
                // Authentication service paths
                "authentication" when originalPath.StartsWith("/api/account") =>
                    originalPath.Replace("/api/account", "/auth"),
                "authentication" when originalPath.StartsWith("/api/users") =>
                    originalPath.Replace("/api/users", "/auth/users"),
                "authentication" when originalPath.StartsWith("/api/roles") =>
                    originalPath.Replace("/api/roles", "/auth/roles"),

                // Patient service
                "patient" when originalPath.StartsWith("/api/patients") =>
                    originalPath.Replace("/api/patients", "/patients"),
                "patient" => "/patients" + originalPath.Replace("/api/", "/"),

                // Doctor service
                "doctor" when originalPath.StartsWith("/api/doctors") =>
                    originalPath.Replace("/api/doctors", "/doctors"),
                "doctor" => "/doctors" + originalPath.Replace("/api/", "/"),

                // Staff service
                "staff" when originalPath.StartsWith("/api/staff") =>
                    originalPath.Replace("/api/staff", "/staff"),
                "staff" => "/staff" + originalPath.Replace("/api/", "/"),

                // Appointment service
                "appointment" when originalPath.StartsWith("/api/appointments") =>
                    originalPath.Replace("/api/appointments", "/appointments"),
                "appointment" => "/appointments" + originalPath.Replace("/api/", "/"),

                // Medical Records service
                "medicalrecords" when originalPath.StartsWith("/api/medical-records") =>
                    originalPath.Replace("/api/medical-records", "/medical-records"),
                "medicalrecords" => "/medical-records" + originalPath.Replace("/api/", "/"),

                // Billing service
                "billing" when originalPath.StartsWith("/api/billing") =>
                    originalPath.Replace("/api/billing", "/billing"),
                "billing" => "/billing" + originalPath.Replace("/api/", "/"),

                // Pharmacy service
                "pharmacy" when originalPath.StartsWith("/api/pharmacy") =>
                    originalPath.Replace("/api/pharmacy", "/pharmacy"),
                "pharmacy" => "/pharmacy" + originalPath.Replace("/api/", "/"),

                // Laboratory service
                "laboratory" when originalPath.StartsWith("/api/laboratory") =>
                    originalPath.Replace("/api/laboratory", "/laboratory"),
                "laboratory" => "/laboratory" + originalPath.Replace("/api/", "/"),

                _ => originalPath
            };
        }

        private void AddGatewayEndpoints(Dictionary<string, object> paths)
        {
            // Aggregation endpoints
            paths["/api/aggregation/patient-dashboard/{patientId}"] = new
            {
                get = new
                {
                    tags = new[] { "Gateway - Aggregation" },
                    summary = "Get patient dashboard",
                    description = "Aggregates data from Patient, Appointment, Medical Records, and Billing services",
                    operationId = "GetPatientDashboard",
                    parameters = new[]
                    {
                        new {
                            name = "patientId",
                            @in = "path",
                            required = true,
                            schema = new { type = "string", format = "uuid" },
                            description = "The unique identifier of the patient"
                        }
                    },
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new
                        {
                            description = "Success",
                            content = new Dictionary<string, object>
                            {
                                ["application/json"] = new
                                {
                                    schema = new
                                    {
                                        type = "object",
                                        properties = new Dictionary<string, object>
                                        {
                                            ["patient"] = new { type = "object", description = "Patient information" },
                                            ["appointments"] = new { type = "array", description = "Patient appointments" },
                                            ["medicalRecords"] = new { type = "array", description = "Medical records" },
                                            ["billings"] = new { type = "array", description = "Billing information" },
                                            ["timestamp"] = new { type = "string", format = "date-time" }
                                        }
                                    }
                                }
                            }
                        },
                        ["401"] = new { description = "Unauthorized" },
                        ["404"] = new { description = "Patient not found" },
                        ["500"] = new { description = "Internal server error" }
                    },
                    security = new[] { new { Bearer = Array.Empty<string>() } }
                }
            };

            paths["/api/aggregation/doctor-dashboard/{doctorId}"] = new
            {
                get = new
                {
                    tags = new[] { "Gateway - Aggregation" },
                    summary = "Get doctor dashboard",
                    description = "Aggregates doctor information, today's appointments, and patient list",
                    operationId = "GetDoctorDashboard",
                    parameters = new[]
                    {
                        new {
                            name = "doctorId",
                            @in = "path",
                            required = true,
                            schema = new { type = "string", format = "uuid" }
                        }
                    },
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new { description = "Success" },
                        ["401"] = new { description = "Unauthorized" },
                        ["404"] = new { description = "Doctor not found" }
                    },
                    security = new[] { new { Bearer = Array.Empty<string>() } }
                }
            };

            paths["/api/aggregation/appointment-details/{appointmentId}"] = new
            {
                get = new
                {
                    tags = new[] { "Gateway - Aggregation" },
                    summary = "Get appointment details",
                    description = "Get appointment with patient and doctor information",
                    operationId = "GetAppointmentDetails",
                    parameters = new[]
                    {
                        new {
                            name = "appointmentId",
                            @in = "path",
                            required = true,
                            schema = new { type = "string", format = "uuid" }
                        }
                    },
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new { description = "Success" },
                        ["401"] = new { description = "Unauthorized" },
                        ["404"] = new { description = "Appointment not found" }
                    },
                    security = new[] { new { Bearer = Array.Empty<string>() } }
                }
            };

            paths["/api/aggregation/hospital-statistics"] = new
            {
                get = new
                {
                    tags = new[] { "Gateway - Aggregation" },
                    summary = "Get hospital statistics",
                    description = "Admin-only endpoint to get comprehensive hospital statistics",
                    operationId = "GetHospitalStatistics",
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new { description = "Success" },
                        ["401"] = new { description = "Unauthorized" },
                        ["403"] = new { description = "Forbidden - Admin role required" }
                    },
                    security = new[] { new { Bearer = Array.Empty<string>() } }
                }
            };

            // Health check endpoints
            paths["/api/healthcheck/services"] = new
            {
                get = new
                {
                    tags = new[] { "Gateway - Health" },
                    summary = "Check health of all microservices",
                    operationId = "CheckAllServices",
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new
                        {
                            description = "Health status of all services",
                            content = new Dictionary<string, object>
                            {
                                ["application/json"] = new
                                {
                                    schema = new
                                    {
                                        type = "object",
                                        properties = new Dictionary<string, object>
                                        {
                                            ["gateway"] = new { type = "string" },
                                            ["overallStatus"] = new { type = "string" },
                                            ["services"] = new { type = "object" },
                                            ["timestamp"] = new { type = "string", format = "date-time" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            paths["/api/healthcheck/service/{serviceName}"] = new
            {
                get = new
                {
                    tags = new[] { "Gateway - Health" },
                    summary = "Check specific service health",
                    operationId = "CheckService",
                    parameters = new[]
                    {
                        new {
                            name = "serviceName",
                            @in = "path",
                            required = true,
                            schema = new { type = "string" },
                            description = "Name of the service to check"
                        }
                    },
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new { description = "Service health status" },
                        ["404"] = new { description = "Service not found" }
                    }
                }
            };

            paths["/api/healthcheck/info"] = new
            {
                get = new
                {
                    tags = new[] { "Gateway - Health" },
                    summary = "Get gateway information",
                    operationId = "GetGatewayInfo",
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new { description = "Gateway information" }
                    }
                }
            };
        }
    }
}