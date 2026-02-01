using HMS.Appointment.Application.DTOs;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Appointment.Application.Commands
{
    public class CreateAppointmentCommand : IRequest<Result<CreateAppointmentResponse>>
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public int DurationMinutes { get; set; } = 30;
        public string AppointmentType { get; set; } = "Consultation";
        public string Priority { get; set; } = "Routine";
        public string? ChiefComplaint { get; set; }
        public string? Notes { get; set; }
        public bool IsInsuranceCovered { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
