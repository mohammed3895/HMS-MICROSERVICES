using HMS.Appointment.Application.DTOs;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class UpdateDoctorScheduleCommand : IRequest<Result<bool>>
    {
        public Guid DoctorId { get; set; }
        public List<ScheduleSlot> Schedule { get; set; } = new();
        public Guid UpdatedBy { get; set; }
    }
}
