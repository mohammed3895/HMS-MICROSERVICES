namespace HMS.Web.Models.DTOs.Patient
{
    public class PatientDto
    {
        public Guid PatientId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
