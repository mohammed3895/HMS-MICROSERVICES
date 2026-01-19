namespace HMS.Web.Controllers
{
    using HMS.Web.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IAuthService _authService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(
            IDoctorService doctorService,
            IAuthService authService,
            ILogger<DoctorController> logger)
        {
            _doctorService = doctorService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // ✅ FIXED: Removed token parameter
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return View(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctors");
                TempData["Error"] = "Failed to load doctors";
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // ✅ FIXED: Removed token parameter
                var doctor = await _doctorService.GetDoctorAsync(id);
                return View(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctor details");
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> BySpecialization(string specialization)
        {
            try
            {
                // ✅ FIXED: Removed token parameter
                var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specialization);
                ViewBag.Specialization = specialization;
                return View("Index", doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctors by specialization");
                return NotFound();
            }
        }
    }
}