using HMS.Common.DTOs;
using HMS.Staff.Domain.Enums;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class ApplyDoctorLeaveCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public bool RescheduleExistingAppointments { get; set; } = true;
    }
}
