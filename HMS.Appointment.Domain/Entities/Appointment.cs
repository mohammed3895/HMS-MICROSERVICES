
using HMS.Appointment.Domain.Enums;
using HMS.Staff.Domain.Entities;

namespace HMS.Appointment.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public string AppointmentNumber { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentType Type { get; set; } // Consultation, Follow-up, Procedure
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? Symptoms { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsUrgent { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public HMS.Staff.Domain.Entities.Staff Doctor { get; set; }
        public Department Department { get; set; }
        public ICollection<AppointmentSlot> Slots { get; set; }
    }
}
