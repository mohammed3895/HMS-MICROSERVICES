namespace HMS.Web.Models.DTOs.Doctor
{
    public class DoctorDto
    {
        public Guid DoctorId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
