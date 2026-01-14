using HMS.Billing.Domain.Enums;

namespace HMS.Billing.Domain.Entities
{
    public class InsuranceClaim
    {
        public Guid Id { get; set; }
        public string ClaimNumber { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid PatientId { get; set; }
        public string InsuranceProvider { get; set; }
        public string PolicyNumber { get; set; }
        public string? GroupNumber { get; set; }
        public decimal ClaimAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public decimal PatientResponsibility { get; set; }
        public InsuranceStatus Status { get; set; }
        public DateTime ClaimDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? DenialReason { get; set; }
        public string? Notes { get; set; }
        public string? AttachmentPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Invoice Invoice { get; set; }
    }
}
