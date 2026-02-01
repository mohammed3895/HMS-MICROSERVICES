using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class BulkRescheduleAppointmentsCommand : IRequest<Result<int>>
    {
        public Guid DoctorId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Guid RescheduledBy { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
