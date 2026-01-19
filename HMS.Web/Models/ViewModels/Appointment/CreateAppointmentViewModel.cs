namespace HMS.Web.Models.ViewModels.Appointment
{
    public class CreateAppointmentViewModel
    {
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
    }
}
