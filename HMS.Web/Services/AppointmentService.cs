using HMS.Web.Interfaces;
using HMS.Web.Models.DTOs.Appointment;
using HMS.Web.Models.ViewModels.Appointment;

namespace HMS.Web.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IApiClientService _apiClient;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IApiClientService apiClient, ILogger<AppointmentService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId)
        {
            try
            {
                var appointments = await _apiClient.GetAsync<List<AppointmentDto>>(
                    $"/appointments/patient/{patientId}");

                return appointments ?? new List<AppointmentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient appointments for {PatientId}", patientId);
                throw;
            }
        }

        public async Task<AppointmentDetailsViewModel> GetAppointmentDetailsAsync(Guid appointmentId)
        {
            try
            {
                var details = await _apiClient.GetAsync<AppointmentDetailsViewModel>(
                    $"/api/aggregation/appointment-details/{appointmentId}");

                return details ?? new AppointmentDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment details for {AppointmentId}", appointmentId);
                throw;
            }
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentViewModel model)
        {
            try
            {
                var appointment = await _apiClient.PostAsync<AppointmentDto>(
                    "/appointments",
                    model);

                return appointment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                throw;
            }
        }

        public async Task<AppointmentDto> UpdateAppointmentAsync(Guid appointmentId, AppointmentDto model)
        {
            try
            {
                var appointment = await _apiClient.PutAsync<AppointmentDto>(
                    $"/appointments/{appointmentId}",
                    model);

                return appointment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {AppointmentId}", appointmentId);
                throw;
            }
        }

        public async Task CancelAppointmentAsync(Guid appointmentId)
        {
            try
            {
                await _apiClient.DeleteAsync<object>($"/appointments/{appointmentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling appointment {AppointmentId}", appointmentId);
                throw;
            }
        }
    }
}
