using HMS.Web.Models.DTOs.Appointment;
using HMS.Web.Models.ViewModels.Appointment;

namespace HMS.Web.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId);
        Task<AppointmentDetailsViewModel> GetAppointmentDetailsAsync(Guid appointmentId);
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentViewModel model);
        Task<AppointmentDto> UpdateAppointmentAsync(Guid appointmentId, AppointmentDto model);
        Task CancelAppointmentAsync(Guid appointmentId);
    }
}