using HMS.Appointment.Application.DTOs;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Queries
{
    public class GetDoctorAppointmentsQuery : IRequest<Result<List<AppointmentDetailsDto>>>
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
    }
}
