using Fido2NetLib;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Configuration;
using HMS.Authentication.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HMS.Authentication.Infrastructure.ServiceExtensions
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<AuthenticationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("AuthenticationDb"),
                    b => b.MigrationsAssembly("HMS.Authentication.Infrastructure")));

            // Configure Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 4;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailTokenProvider<ApplicationUser>>("Email");

            // Configure JWT Settings
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                ?? throw new InvalidOperationException("JwtSettings configuration is missing");
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Configure Email Settings
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>()
                ?? throw new InvalidOperationException("EmailSettings configuration is missing");
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Configure Security Settings
            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));

            // Configure WebAuthn Settings (optional)
            var webAuthnOrigin = configuration["WebAuthnSettings:Origin"];
            if (!string.IsNullOrEmpty(webAuthnOrigin))
            {
                var webAuthnSettings = configuration.GetSection("WebAuthnSettings").Get<WebAuthnSettings>();
                if (webAuthnSettings != null)
                {
                    services.Configure<WebAuthnSettings>(configuration.GetSection("WebAuthnSettings"));

                    // Add Fido2 (WebAuthn) - Register as IFido2 interface
                    var fido2Configuration = new Fido2Configuration
                    {
                        ServerDomain = new Uri(webAuthnSettings.Origin).Host,
                        ServerName = webAuthnSettings.RelyingPartyName,
                        Origins = new HashSet<string> { webAuthnSettings.Origin },
                        TimestampDriftTolerance = configuration.GetValue<int>("WebAuthnSettings:TimestampDriftTolerance", 300000)
                    };

                    services.AddSingleton<IFido2>(new Fido2(fido2Configuration));
                }
            }

            // Configure CORS to allow credentials (cookies)
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
                        .AllowCredentials(); // Important: Allow cookies
                });
            });

            // Add JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    // Allow reading token from cookie as fallback
                    OnMessageReceived = context =>
                    {
                        // Check Authorization header first
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            // Fallback to cookie
                            context.Token = context.Request.Cookies["accessToken"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Configure Cookie Policy
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            // Add Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireDoctorRole", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("RequireNurseRole", policy => policy.RequireRole("Nurse"));
                options.AddPolicy("RequirePatientRole", policy => policy.RequireRole("Patient"));
                options.AddPolicy("RequireStaffRole", policy => policy.RequireRole("Admin", "Doctor", "Nurse"));
            });

            return services;
        }
    }
}