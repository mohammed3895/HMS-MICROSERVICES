using FluentValidation;
using HMS.Authentication.Application.Behaviors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HMS.Authentication.Application.ServiceExtensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.Load("HMS.Authentication.Application"));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            });

            // Add AutoMapper
            services.AddAutoMapper(Assembly.Load("HMS.Authentication.Application"));

            // Add FluentValidation
            services.AddValidatorsFromAssembly(Assembly.Load("HMS.Authentication.Application"));

            // Add HttpContextAccessor
            services.AddHttpContextAccessor();

            return services;
        }
    }
}