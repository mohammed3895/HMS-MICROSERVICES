using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class RescheduleAppointmentCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public DateTime NewAppointmentDate { get; set; }
        public TimeSpan NewStartTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid RescheduledBy { get; set; }
        public bool NotifyPatient { get; set; } = true;
    }
}
