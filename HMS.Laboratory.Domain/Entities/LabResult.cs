namespace HMS.Laboratory.Domain.Entities
{
    public class LabResult
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        // Result Details
        public string ResultData { get; set; } // JSON structured data
        public string? Interpretation { get; set; }
        public string? Notes { get; set; }
        public bool IsAbnormal { get; set; }
        public bool IsCritical { get; set; }
        public string? AbnormalFlags { get; set; } // H (High), L (Low), * (Abnormal)
        public string? CriticalRange { get; set; }
        public string? ReferenceRange { get; set; }
        public string? Units { get; set; }
        public string? Method { get; set; } // Testing method used
        public string? Equipment { get; set; } // Equipment used

        // Status
        public ResultStatus Status { get; set; }
        public DateTime ResultDate { get; set; }

        // Verification
        public Guid? VerifiedBy { get; set; }
        public DateTime? VerificationDate { get; set; }

        // Reporting
        public string? ReportFormat { get; set; } // PDF, HTML, XML
        public string? ReportPath { get; set; }
        public bool IsReportGenerated { get; set; }
        public DateTime? ReportGenerationDate { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public LabOrder Order { get; set; }
        public ICollection<LabResultAttachment> Attachments { get; set; } = new List<LabResultAttachment>();
    }
}