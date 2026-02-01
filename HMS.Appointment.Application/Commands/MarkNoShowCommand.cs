using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class MarkNoShowCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public Guid MarkedBy { get; set; }
        public string? Notes { get; set; }
    }
}
