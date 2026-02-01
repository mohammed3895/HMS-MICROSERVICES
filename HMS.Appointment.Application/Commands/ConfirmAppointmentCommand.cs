using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class ConfirmAppointmentCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public string ConfirmationMethod { get; set; } = "Phone"; // Phone, SMS, Email, Portal
    }
}
