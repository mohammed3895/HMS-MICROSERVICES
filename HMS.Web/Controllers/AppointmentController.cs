namespace HMS.Web.Controllers
{
    using HMS.Web.Interfaces;
    using HMS.Web.Models.ViewModels.Appointment;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IAuthService _authService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            IAuthService authService,
            ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                //var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                //if (!Guid.TryParse(userId, out var patientId))
                //    return RedirectToAction("Login", "Auth");

                //var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointments");
                TempData["Error"] = "Failed to load appointments";
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                // ✅ FIXED: Removed token parameter
                var doctors = await _doctorService.GetAllDoctorsAsync();
                ViewBag.Doctors = doctors;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create appointment");
                TempData["Error"] = "Failed to load doctors";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // ✅ FIXED: Removed token parameter
                await _appointmentService.CreateAppointmentAsync(model);

                TempData["Success"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                TempData["Error"] = "Failed to create appointment";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                // ✅ FIXED: Removed token parameter
                var appointmentDetails = await _appointmentService.GetAppointmentDetailsAsync(id);

                return View(appointmentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment details");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                // ✅ FIXED: Removed token parameter
                await _appointmentService.CancelAppointmentAsync(id);

                TempData["Success"] = "Appointment cancelled successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment");
                TempData["Error"] = "Failed to cancel appointment";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}