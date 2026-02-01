using HMS.Appointment.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace HMS.Appointment.Infrastructure.ServiceExtensions
{
    public static class AppointmentServiceExtensions
    {
        public static IServiceCollection AddAppointmentServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<AppointmentDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("AppointmentDb"),
                    b => b.MigrationsAssembly("HMS.Appointment.Infrastructure")));

            // Add MediatR for CQRS
            services.AddMediatR(cfg =>
            {
                // Register all handlers from the Application assembly
                cfg.RegisterServicesFromAssembly(
                    Assembly.Load("HMS.Appointment.Application"));
            });

            // JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"]
                ?? throw new InvalidOperationException("JWT Secret is not configured");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Add Authorization Policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole",
                    policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireDoctorRole",
                    policy => policy.RequireRole("Doctor"));
                options.AddPolicy("RequireReceptionistRole",
                    policy => policy.RequireRole("Receptionist"));
                options.AddPolicy("RequireStaffRole",
                    policy => policy.RequireRole("Admin", "Doctor", "Nurse", "Receptionist"));
            });

            // Add CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["CorsSettings:AllowedOrigins"]?.Split(',')
                            ?? new[] { "http://localhost:3000", "https://localhost:3000" }
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // Add HttpClient for inter-service communication
            services.AddHttpClient("AuthenticationService", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ServiceEndpoints:Authentication"]
                    ?? "https://localhost:5001");
            });

            services.AddHttpClient("PatientService", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ServiceEndpoints:Patient"]
                    ?? "https://localhost:5002");
            });

            services.AddHttpClient("DoctorService", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ServiceEndpoints:Doctor"]
                    ?? "https://localhost:5003");
            });

            services.AddHttpClient("NotificationService", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ServiceEndpoints:Notification"]
                    ?? "https://localhost:5010");
            });

            services.AddHttpClient("BillingService", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ServiceEndpoints:Billing"]
                    ?? "https://localhost:5007");
            });

            // Add Memory Cache for time slots
            services.AddMemoryCache();

            // Add Background Services (uncomment when implemented)
            // services.AddHostedService<AppointmentReminderService>();
            // services.AddHostedService<WaitlistProcessorService>();

            return services;
        }
    }
}