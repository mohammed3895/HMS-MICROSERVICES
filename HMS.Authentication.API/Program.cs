using HMS.Authentication.API.Filters;
using HMS.Authentication.Application.ServiceExtensions;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.ServiceExtensions;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Services.AddLoggingServices(builder.Configuration, builder.Environment);
builder.Host.UseSerilog();

// Add Services
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Controllers
builder.Services.AddControllers();

// Add API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HMS Authentication API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme",
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
    c.OperationFilter<FileUploadOperationFilter>();
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "https://localhost:5000",
                "https://localhost:7047",
                "http://localhost:5000",
                "http://localhost:7047")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "https://localhost:5000",
                "https://localhost:7047",
                "https://yourdomain.com",
                "https://www.yourdomain.com",
                "https://admin.yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AuthenticationDbContext>();

var app = builder.Build();

// Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.Services.InitializeDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

// ✅ CORS before Authentication
app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

app.UseRateLimiter();

// ✅ CORRECT MIDDLEWARE ORDER
app.UseRouting();

// ✅ AUTHENTICATION BEFORE AUTHORIZATION
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

try
{
    Log.Information("Starting HMS Authentication API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}