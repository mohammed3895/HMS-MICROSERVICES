using HMS.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IAuthService _authService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IPatientService patientService,
            IDoctorService doctorService,
            IAuthService authService,
            ILogger<DashboardController> logger)
        {
            _patientService = patientService;
            _doctorService = doctorService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // ✅ Check if user is actually authenticated
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    _logger.LogWarning("Unauthenticated access attempt to dashboard");
                    return RedirectToAction("Login", "Auth");
                }

                // ✅ Check if session has valid token
                var token = await _authService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No access token found in session, redirecting to login");
                    await _authService.LogoutAsync(); // Clear invalid session
                    TempData["Error"] = "Your session has expired. Please log in again.";
                    return RedirectToAction("Login", "Auth");
                }

                var dashboard = await _patientService.GetDashboardAsync();
                return View(dashboard);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("Unauthorized"))
            {
                // ✅ Handle 401 Unauthorized specifically
                _logger.LogWarning(ex, "Unauthorized access to dashboard - clearing session");
                await _authService.LogoutAsync();
                TempData["Error"] = "Your session has expired. Please log in again.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = "Failed to load dashboard";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}