using HMS.Web.Interfaces;
using HMS.Web.Middleware;
using HMS.Web.Policies;
using HMS.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/web-log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "HMS.Web")
    .CreateLogger();

builder.Host.UseSerilog();

// Add Configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var apiGatewayUrl = builder.Configuration.GetValue<string>("ApiGateway:BaseUrl") ?? "https://localhost:7047";

// Add Authentication (Cookie-based for MVC)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/home/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// ✅ ADD AUTHORIZATION
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireDoctor", policy =>
        policy.RequireRole("Doctor"));

    options.AddPolicy("RequirePatient", policy =>
        policy.RequireRole("Patient"));

    options.AddPolicy("RequireStaff", policy =>
        policy.RequireRole("Staff"));
});

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Add HttpClient with Polly
builder.Services.AddHttpClient("ApiGateway", client =>
{
    client.BaseAddress = new Uri(apiGatewayUrl);
    client.DefaultRequestHeaders.Add("User-Agent", "HMS-Web-Client");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(HttpClientPolicies.GetRetryPolicy(null))
.AddPolicyHandler(HttpClientPolicies.GetCircuitBreakerPolicy(null))
.ConfigurePrimaryHttpMessageHandler(() =>
{
    if (builder.Environment.IsDevelopment())
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
    }
    return new HttpClientHandler();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<Lazy<IApiClientService>>(provider =>
    new Lazy<IApiClientService>(() => provider.GetRequiredService<IApiClientService>()));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();

// Add MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins(
                "https://localhost:5000",
                "https://localhost:5001",
                "https://localhost:7047",
                "http://localhost:5000",
                "http://localhost:5001",
                "http://localhost:7047")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowCredentials()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/home/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

// ✅ CORRECT MIDDLEWARE ORDER
app.UseRouting();

// Session BEFORE Authentication
app.UseSession();

// Authentication BEFORE Authorization
app.UseAuthentication();

// Authorization AFTER Authentication
app.UseAuthorization();

// Custom middleware
app.UseMiddleware<JwtTokenMiddleware>();

// Error handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        var exception = exceptionHandlerPathFeature?.Error;

        Log.Error(exception, "Unhandled exception");

        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred",
            traceId = context.TraceIdentifier
        });
    });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

try
{
    Log.Information("Starting HMS Web Application");
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