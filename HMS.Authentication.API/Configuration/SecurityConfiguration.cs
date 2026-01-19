using HMS.Authentication.Infrastructure.Authorization.Handlers;
using HMS.Authentication.Infrastructure.Authorization.Polices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace HMS.Authentication.API.Configuration
{
    public static class SecurityConfiguration
    {
        public static IServiceCollection AddSecurityServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add authorization
            services.AddAuthorization(options =>
            {
                AuthorizationPolicyConfiguration
                    .ConfigurePolicies(options);
            });

            // Register authorization handlers
            services.AddScoped<IAuthorizationHandler,
                EmailConfirmedAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler,
               ActiveAccountAuthorizationHandler>();

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowWebApp", builder =>
                {
                    builder
                        .WithOrigins(configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" })
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // Add rate limiting (if using AspNetCore.RateLimiting)
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("login", options =>
                {
                    options.PermitLimit = 5;
                    options.Window = TimeSpan.FromMinutes(15);
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("register", options =>
                {
                    options.PermitLimit = 3;
                    options.Window = TimeSpan.FromHours(1);
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("otp", options =>
                {
                    options.PermitLimit = 10;
                    options.Window = TimeSpan.FromMinutes(15);
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });
            });

            return services;
        }

        public static WebApplication UseSecurityMiddleware(this WebApplication app)
        {
            // Use CORS
            app.UseCors("AllowWebApp");

            // Use rate limiting
            app.UseRateLimiter();

            // Use authentication & authorization
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
