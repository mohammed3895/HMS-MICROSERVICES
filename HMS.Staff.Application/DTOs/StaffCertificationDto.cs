namespace HMS.Staff.Application.DTOs
{
    public class StaffCertificationDto
    {
        public Guid Id { get; set; }
        public string CertificationName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
