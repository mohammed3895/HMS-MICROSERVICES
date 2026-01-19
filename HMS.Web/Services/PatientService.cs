using HMS.Web.Interfaces;
using HMS.Web.Models.DTOs.Patient;
using HMS.Web.Models.ViewModels.Patient;

namespace HMS.Web.Services
{
    public class PatientService : IPatientService
    {
        private readonly IApiClientService _apiClient;
        private readonly IAuthService _authService;
        private readonly ILogger<PatientService> _logger;

        public PatientService(
            IApiClientService apiClient,
            IAuthService authService,
            ILogger<PatientService> logger)
        {
            _apiClient = apiClient;
            _authService = authService;
            _logger = logger;
        }

        public async Task<PatientDashboardViewModel> GetDashboardAsync()
        {
            try
            {
                var userId = _authService.GetCurrentUserId() ?? Guid.NewGuid();

                var dashboard = await _apiClient.GetAsync<PatientDashboardViewModel>(
                    $"/api/aggregation/patient-dashboard/{userId}");

                return dashboard ?? new PatientDashboardViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient dashboard");
                throw;
            }
        }

        public async Task<PatientListViewModel> GetPatientsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var patients = await _apiClient.GetAsync<PatientListViewModel>(
                    $"/patients?page={page}&pageSize={pageSize}");

                return patients ?? new PatientListViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patients list");
                throw;
            }
        }

        public async Task<PatientDto> GetPatientAsync(Guid patientId)
        {
            try
            {
                var patient = await _apiClient.GetAsync<PatientDto>(
                    $"/patients/{patientId}");

                return patient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient {PatientId}", patientId);
                throw;
            }
        }

        public async Task<PatientDto> UpdatePatientAsync(Guid patientId, PatientDto model)
        {
            try
            {
                var patient = await _apiClient.PutAsync<PatientDto>(
                    $"/patients/{patientId}",
                    model);

                return patient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId}", patientId);
                throw;
            }
        }
    }
}