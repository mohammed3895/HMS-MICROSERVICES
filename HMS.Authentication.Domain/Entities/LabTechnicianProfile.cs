namespace HMS.Authentication.Domain.Entities
{
    public class LabTechnicianProfile : BaseStaffProfile
    {
        public string? LabSection { get; set; }
        public List<string> Certifications { get; set; } = new();
    }
}
