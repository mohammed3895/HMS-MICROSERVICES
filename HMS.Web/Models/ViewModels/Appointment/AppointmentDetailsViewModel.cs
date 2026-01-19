using HMS.Web.Models.DTOs.Appointment;
using HMS.Web.Models.DTOs.Doctor;
using HMS.Web.Models.DTOs.Patient;

namespace HMS.Web.Models.ViewModels.Appointment
{
    public class AppointmentDetailsViewModel
    {
        public AppointmentDto Appointment { get; set; }
        public PatientDto Patient { get; set; }
        public DoctorDto Doctor { get; set; }
    }
}
