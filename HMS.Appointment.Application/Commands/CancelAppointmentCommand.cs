using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class CancelAppointmentCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public string CancellationReason { get; set; } = string.Empty;
        public Guid CancelledBy { get; set; }
        public bool NotifyPatient { get; set; } = true;
        public bool AllowRefund { get; set; } = false;
    }
}
