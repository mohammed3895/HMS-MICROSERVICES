using HMS.Web.Models.DTOs.Appointment;
using HMS.Web.Models.DTOs.Doctor;
using HMS.Web.Models.DTOs.Patient;

namespace HMS.Web.Models.ViewModels.Doctor
{
    public class DoctorDashboardViewModel
    {
        public DoctorDto Doctor { get; set; }
        public List<AppointmentDto> TodayAppointments { get; set; }
        public List<PatientDto> Patients { get; set; }
    }
}
