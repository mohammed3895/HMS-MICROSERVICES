using Microsoft.AspNetCore.Mvc;

namespace HMS.APIGateway.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Gateway welcome endpoint - accessible at root "/"
        /// </summary>
        [HttpGet("/")]
        public IActionResult Index()
        {
            return Ok(new
            {
                service = "HMS API Gateway",
                version = "1.0.0",
                status = "Running",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                documentation = new
                {
                    swagger = "/swagger",
                    postman = "Import the Postman collection for API testing"
                },
                health = new
                {
                    gateway = "/health",
                    allServices = "/api/healthcheck/services",
                    specificService = "/api/healthcheck/service/{serviceName}"
                },
                endpoints = new
                {
                    authentication = new
                    {
                        register = "POST /auth/register - Register new patient account (PUBLIC)",
                        confirmEmail = "POST /auth/confirm-email - Confirm email address (PUBLIC)",
                        login = "POST /auth/login - Login to system (PUBLIC)",
                        logout = "POST /auth/logout - Logout from system",
                        changePassword = "POST /auth/change-password - Change password",
                        resetPassword = "POST /auth/reset-password - Reset forgotten password",
                        twoFactor = "POST /auth/2fa/* - Two-factor authentication endpoints",
                        users = "/auth/users/* - User management (ADMIN)",
                        roles = "/auth/roles/* - Role management (ADMIN)"
                    },
                    services = new
                    {
                        patients = "/patients/* - Patient management",
                        doctors = "/doctors/* - Doctor management",
                        staff = "/staff/* - Staff management",
                        appointments = "/appointments/* - Appointment scheduling",
                        medicalRecords = "/medical-records/* - Medical records",
                        billing = "/billing/* - Billing and payments",
                        pharmacy = "/pharmacy/* - Pharmacy services",
                        laboratory = "/laboratory/* - Laboratory services",
                        notifications = "/notifications/* - Notifications"
                    },
                    aggregation = new
                    {
                        patientDashboard = "GET /api/aggregation/patient-dashboard/{patientId}",
                        doctorDashboard = "GET /api/aggregation/doctor-dashboard/{doctorId}",
                        appointmentDetails = "GET /api/aggregation/appointment-details/{appointmentId}",
                        hospitalStatistics = "GET /api/aggregation/hospital-statistics - Admin only"
                    }
                },
                quickStart = new
                {
                    newPatient = new
                    {
                        step1 = "Register: POST /auth/register with email, password, name, phone, and date of birth",
                        step2 = "Confirm Email: Check your email for confirmation link (if required)",
                        step3 = "Login: POST /auth/login with your email and password",
                        step4 = "Access Dashboard: GET /api/aggregation/patient-dashboard/{patientId} with your token"
                    },
                    existingUser = new
                    {
                        step1 = "Login: POST /auth/login with email and password",
                        step2 = "Copy the accessToken from the response",
                        step3 = "Use the token: Add 'Authorization: Bearer {token}' header to your requests",
                        step4 = "Access services: GET /patients, /doctors, /appointments, etc."
                    }
                },
                publicEndpoints = new[]
                {
                    "POST /auth/register - Create new patient account",
                    "POST /auth/login - Login to system",
                    "POST /auth/confirm-email - Confirm email address",
                    "POST /auth/request-password-reset - Request password reset",
                    "POST /auth/reset-password - Reset password with token",
                    "GET /health - Gateway health check",
                    "GET /api/healthcheck/services - All services health"
                },
                securityNote = "Most endpoints require JWT authentication. Include 'Authorization: Bearer {token}' header in your requests."
            });
        }

        /// <summary>
        /// Gateway information - accessible at "/info"
        /// </summary>
        [HttpGet("/info")]
        public IActionResult Info()
        {
            var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();

            return Ok(new
            {
                gateway = new
                {
                    name = "HMS API Gateway",
                    version = "1.0.0",
                    description = "Unified API Gateway for Hospital Management System Microservices"
                },
                system = new
                {
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    uptime = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                    dotnetVersion = Environment.Version.ToString(),
                    osVersion = Environment.OSVersion.ToString(),
                    machineName = Environment.MachineName
                },
                features = new[]
                {
                    "JWT Authentication",
                    "Patient Self-Registration",
                    "Email Confirmation",
                    "Rate Limiting",
                    "Response Caching",
                    "Load Balancing",
                    "Health Monitoring",
                    "Request Aggregation",
                    "Distributed Tracing",
                    "Structured Logging"
                },
                authentication = new
                {
                    publicRegistration = true,
                    emailConfirmation = "Optional - Check SecuritySettings:RequireConfirmedEmail",
                    twoFactorAuth = "Supported - TOTP and WebAuthn",
                    passwordReset = true,
                    tokenExpiry = "1 hour (configurable)",
                    refreshTokenExpiry = "7 days (configurable)"
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}