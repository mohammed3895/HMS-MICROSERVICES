using HMS.Appointment.Application.DTOs;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Queries
{
    public class GetAvailableTimeSlotsQuery : IRequest<Result<List<TimeSlotDto>>>
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
    }
}
