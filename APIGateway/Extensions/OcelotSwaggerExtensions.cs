using Microsoft.OpenApi.Models;

namespace APIGateway.Extensions
{
    public static class OcelotSwaggerExtensions
    {
        public static IServiceCollection AddOcelotSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HMS API Gateway - Complete API Documentation",
                    Version = "v1",
                    Description = "Unified API Gateway for Hospital Management System with all downstream services"
                });

                // Define all Ocelot routes as tags
                var routes = configuration.GetSection("Routes").Get<List<OcelotRoute>>();
                if (routes != null)
                {
                    foreach (var route in routes)
                    {
                        var tagName = GetServiceNameFromPath(route.UpstreamPathTemplate);
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            c.TagActionsBy(api => new[] { tagName });
                        }
                    }
                }

                // Add security definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

                // Add gateway-specific endpoints
                c.TagActionsBy(api =>
                {
                    if (api.RelativePath.StartsWith("api/aggregation"))
                        return new[] { "Gateway - Aggregation" };
                    if (api.RelativePath.StartsWith("api/healthcheck"))
                        return new[] { "Gateway - Health" };
                    return new[] { "Gateway" };
                });
            });

            return services;
        }

        private static string GetServiceNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "Other";

            var segments = path.Split('/');
            if (segments.Length > 1)
            {
                var service = segments[1].Replace("-", " ").Replace("{everything}", "");
                if (!string.IsNullOrEmpty(service))
                    return char.ToUpper(service[0]) + service.Substring(1);
            }
            return "Other";
        }

        private class OcelotRoute
        {
            public string UpstreamPathTemplate { get; set; } = string.Empty;
            public string DownstreamPathTemplate { get; set; } = string.Empty;
        }
    }
}
