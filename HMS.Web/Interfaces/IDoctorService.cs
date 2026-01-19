using HMS.Web.Models.DTOs.Doctor;
using HMS.Web.Models.ViewModels.Doctor;

namespace HMS.Web.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorDashboardViewModel> GetDashboardAsync();
        Task<List<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto> GetDoctorAsync(Guid doctorId);
        Task<List<DoctorDto>> GetDoctorsBySpecializationAsync(string specialization);
    }
}