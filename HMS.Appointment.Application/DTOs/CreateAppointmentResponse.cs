namespace HMS.Appointment.Application.DTOs
{
    public class CreateAppointmentResponse
    {
        public Guid AppointmentId { get; set; }
        public string AppointmentNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public decimal ConsultationFee { get; set; }
    }
}
