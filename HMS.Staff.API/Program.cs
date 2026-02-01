using HMS.Staff.Application.Interfaces;
using HMS.Staff.Application.Services;
using HMS.Staff.Infrastructure.Data;
using HMS.Staff.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/staff-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HMS Staff Service API",
        Version = "v1",
        Description = "Hospital Management System - Staff Service API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<StaffDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("HMS.Staff.Infrastructure")));

// HttpClient for Auth Service
builder.Services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
{
    var authServiceUrl = builder.Configuration["ServiceEndpoints:Authentication"] ?? "https://localhost:5001";
    client.BaseAddress = new Uri(authServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // For development only - accept any SSL certificate
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("HMS.Staff.Application"));
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<StaffDbContext>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS Staff Service API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<StaffDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");

        // Seed data
        logger.LogInformation("Starting data seeding...");
        await app.Services.SeedDatabaseAsync();
        logger.LogInformation("Data seeding completed successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
    }
}

Log.Information("HMS Staff Service starting...");
app.Run();
Log.Information("HMS Staff Service stopped");