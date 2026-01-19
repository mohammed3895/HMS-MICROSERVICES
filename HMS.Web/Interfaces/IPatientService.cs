using HMS.Web.Models.DTOs.Patient;
using HMS.Web.Models.ViewModels.Patient;

namespace HMS.Web.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDashboardViewModel> GetDashboardAsync();
        Task<PatientListViewModel> GetPatientsAsync(int page = 1, int pageSize = 10);
        Task<PatientDto> GetPatientAsync(Guid patientId);
        Task<PatientDto> UpdatePatientAsync(Guid patientId, PatientDto model);
    }
}