using HMS.Web.Models.DTOs.Appointment;
using HMS.Web.Models.DTOs.Billing;
using HMS.Web.Models.DTOs.Patient;

namespace HMS.Web.Models.ViewModels.Patient
{
    public class PatientDashboardViewModel
    {
        public PatientDto Patient { get; set; }
        public List<AppointmentDto> Appointments { get; set; }
        public List<MedicalRecordDto> MedicalRecords { get; set; }
        public BillingDto Billing { get; set; }
    }
}
