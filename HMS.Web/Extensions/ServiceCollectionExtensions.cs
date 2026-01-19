using HMS.Web.Interfaces;
using HMS.Web.Services;

namespace HMS.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHmsServices(this IServiceCollection services, IConfiguration configuration)
        {
            // API Gateway Settings
            services.Configure<ApiGatewaySettings>(configuration.GetSection("ApiGateway"));

            // Core Services
            services.AddScoped<IApiClientService, ApiClientService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IAppointmentService, AppointmentService>();

            // HTTP Client
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            // Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = ".HMS.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }
    }
}
