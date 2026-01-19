using HMS.Web.Models.DTOs.Patient;

namespace HMS.Web.Models.ViewModels.Patient
{
    public class PatientListViewModel
    {
        public List<PatientDto> Patients { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
