using HMS.Appointment.Infrastructure.Data;
using HMS.Appointment.Infrastructure.ServiceExtensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Services.AddLoggingServices(builder.Configuration, builder.Environment);
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddAppointmentServices(builder.Configuration);

// Add Controllers
builder.Services.AddControllers();

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "HMS Appointment Management API",
        Version = "v1",
        Description = "API for managing hospital appointments, schedules, and waitlists"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppointmentDbContext>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppointmentDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Ensuring database is created...");

        // Ensure database is created (for development)
        await context.Database.EnsureCreatedAsync();

        logger.LogInformation("Starting database seeding...");

        // Seed data
        await AppointmentDbSeeder.SeedAsync(context, logger);

        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
        // Don't throw - let the app continue even if seeding fails
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS Appointment API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) => ex != null
        ? LogEventLevel.Error
        : httpContext.Response.StatusCode > 499
            ? LogEventLevel.Error
            : LogEventLevel.Information;
});

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

try
{
    Log.Information("Starting HMS Appointment API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Appointment Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}