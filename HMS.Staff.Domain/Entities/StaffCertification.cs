namespace HMS.Staff.Domain.Entities
{
    public class StaffCertification
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string CertificationName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string? CertificateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool NeverExpires { get; set; }
        public string? DocumentUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public Staff Staff { get; set; } = null!;
    }
}
