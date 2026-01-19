using HMS.Web.Interfaces;
using HMS.Web.Models.DTOs.Doctor;
using HMS.Web.Models.ViewModels.Doctor;

namespace HMS.Web.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IApiClientService _apiClient;
        private readonly IAuthService _authService;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IApiClientService apiClient,
            IAuthService authService,
            ILogger<DoctorService> logger)
        {
            _apiClient = apiClient;
            _authService = authService;
            _logger = logger;
        }

        public async Task<DoctorDashboardViewModel> GetDashboardAsync()
        {
            try
            {
                var doctorId = _authService.GetCurrentUserId() ?? Guid.NewGuid();
                var dashboard = await _apiClient.GetAsync<DoctorDashboardViewModel>(
                    $"/api/aggregation/doctor-dashboard/{doctorId}");

                return dashboard ?? new DoctorDashboardViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctor dashboard");
                throw;
            }
        }

        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            try
            {
                var doctors = await _apiClient.GetAsync<List<DoctorDto>>(
                    "/doctors");

                return doctors ?? new List<DoctorDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctors");
                throw;
            }
        }

        public async Task<DoctorDto> GetDoctorAsync(Guid doctorId)
        {
            try
            {
                var doctor = await _apiClient.GetAsync<DoctorDto>(
                    $"/doctors/{doctorId}");

                return doctor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctor {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<List<DoctorDto>> GetDoctorsBySpecializationAsync(string specialization)
        {
            try
            {
                var doctors = await _apiClient.GetAsync<List<DoctorDto>>(
                    $"/doctors/specialization/{specialization}");

                return doctors ?? new List<DoctorDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctors by specialization {Specialization}", specialization);
                throw;
            }
        }
    }
}
