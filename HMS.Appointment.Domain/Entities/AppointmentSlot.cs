using HospitalManagement.Appointment.API.Enums;

namespace HMS.Appointment.Domain.Entities
{
    public class AppointmentSlot
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime SlotDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { set; get; }
        public bool IsAvailable { get; set; }
        public Guid? AppointmentId { get; set; }
        public SlotStatus Status { get; set; }

        public HospitalManagement.Staff.API.Entities.Staff Doctor { get; set; }
        public Appointment Appointment { get; set; }
    }
}
