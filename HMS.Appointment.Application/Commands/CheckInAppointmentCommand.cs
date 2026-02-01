using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class CheckInAppointmentCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public string CheckInMethod { get; set; } = "Reception"; // Kiosk, Reception, Mobile
        public string? AdditionalNotes { get; set; }
    }
}
