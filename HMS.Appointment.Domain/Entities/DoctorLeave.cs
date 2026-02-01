using HMS.Staff.Domain.Enums;

namespace HMS.Appointment.Domain.Entities
{
    public class DoctorLeave
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public LeaveStatus Status { get; set; } // Pending, Approved, Rejected
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool AffectsAppointments { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
