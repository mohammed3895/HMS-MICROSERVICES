using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class CompleteAppointmentCommand : IRequest<Result<bool>>
    {
        public Guid AppointmentId { get; set; }
        public string? ConsultationNotes { get; set; }
        public bool RequiresFollowUp { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public List<string>? Prescriptions { get; set; }
        public List<string>? LabOrders { get; set; }
        public Guid CompletedBy { get; set; }
    }
}
