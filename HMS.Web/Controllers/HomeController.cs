using HMS.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IAuthService authService,
            ILogger<HomeController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // ✅ Check BOTH authentication AND valid token
            if (User.Identity?.IsAuthenticated == true)
            {
                var token = await _authService.GetAccessTokenAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogInformation("Authenticated user with valid token redirected to Dashboard");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    // ✅ Authenticated but no token = expired session
                    _logger.LogWarning("User authenticated but token missing - clearing session");
                    await _authService.LogoutAsync();
                    TempData["Error"] = "Your session has expired. Please log in again.";
                    return RedirectToAction("Login", "Auth");
                }
            }

            // Not authenticated - show home/landing page or redirect to login
            _logger.LogDebug("Unauthenticated user - redirecting to login");
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}