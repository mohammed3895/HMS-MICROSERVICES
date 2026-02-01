using HMS.Staff.Application.Interfaces;
using HMS.Staff.Application.Services;

namespace HMS.Staff.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthServiceClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
            {
                var authServiceUrl = configuration["ServiceEndpoints:Authentication"]
                    ?? "https://localhost:5001";
                client.BaseAddress = new Uri(authServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

            return services;
        }
    }
}
