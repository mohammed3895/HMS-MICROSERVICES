namespace HMS.Appointment.Application.DTOs
{
    public class AppointmentDetailsDto
    {
        public Guid Id { get; set; }
        public string AppointmentNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ChiefComplaint { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public bool IsCheckedIn { get; set; }
    }
}
