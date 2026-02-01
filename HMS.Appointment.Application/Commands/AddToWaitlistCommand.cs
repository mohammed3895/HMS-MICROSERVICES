using HMS.Appointment.Domain.Enums;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class AddToWaitlistCommand : IRequest<Result<Guid>>
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime PreferredDate { get; set; }
        public TimeSpan? PreferredStartTime { get; set; }
        public TimeSpan? PreferredEndTime { get; set; }
        public WaitlistPriority Priority { get; set; } = WaitlistPriority.Medium;
        public string? Notes { get; set; }
    }
}
