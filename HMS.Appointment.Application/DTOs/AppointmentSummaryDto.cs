namespace HMS.Appointment.Application.DTOs
{
    public class AppointmentSummaryDto
    {
        public Guid Id { get; set; }
        public string AppointmentNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
    }
}
