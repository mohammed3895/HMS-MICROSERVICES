using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class SendAppointmentReminderCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public string ReminderType { get; set; } = "SMS"; // SMS, Email, Push
        public string Timing { get; set; } = "24Hours";
    }
}
