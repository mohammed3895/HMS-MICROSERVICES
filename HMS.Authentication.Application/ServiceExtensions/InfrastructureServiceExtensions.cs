using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Authentication.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Authentication.Infrastructure.ServiceExtensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            // Use Memory Cache as Distributed Cache (no Redis needed)
            services.AddDistributedMemoryCache();

            // Register Fido2 for WebAuthn
            services.AddFido2(options =>
            {
                options.ServerDomain = configuration["WebAuthn:RelyingPartyId"] ?? "localhost";
                options.ServerName = configuration["WebAuthn:RelyingPartyName"] ?? "Hospital Management System";
                options.Origins = configuration.GetSection("WebAuthn:Origins").Get<HashSet<string>>()
                    ?? new HashSet<string>
                    {
                        "http://localhost:3000",
                        "https://localhost:3000",
                        "https://localhost:5001",
                        "https://localhost:7047"
                    };
                options.TimestampDriftTolerance = configuration.GetValue<int>("WebAuthn:TimestampDriftTolerance", 300000);
            });

            // Register Services
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IWebAuthnService, WebAuthnService>();

            services.AddScoped<ICacheService, CacheService>();

            services.AddScoped<DatabaseInitializer>();

            return services;
        }
    }
}