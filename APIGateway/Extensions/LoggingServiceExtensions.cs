using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;


namespace HMS.APIGateway.Extensions
{
    public static class LoggingServiceExtensions
    {
        public static IServiceCollection AddLoggingServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "HMS.Authentication")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    path: "logs/auth-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10485760) // 10MB
                .WriteTo.File(
                    path: "logs/auth-error-.txt",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 90)
                .CreateLogger();

            services.AddSerilog();

            return services;
        }
    }
}