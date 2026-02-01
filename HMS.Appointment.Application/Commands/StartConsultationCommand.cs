using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class StartConsultationCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public Guid DoctorId { get; set; }
        public string? RoomNumber { get; set; }
    }
}
