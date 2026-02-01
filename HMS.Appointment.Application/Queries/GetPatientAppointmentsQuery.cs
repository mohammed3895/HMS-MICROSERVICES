using HMS.Appointment.Application.DTOs;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Queries
{
    public class GetPatientAppointmentsQuery : IRequest<Result<List<AppointmentSummaryDto>>>
    {
        public Guid PatientId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
    }
}
