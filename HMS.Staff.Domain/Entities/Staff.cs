using HMS.Staff.Domain.Enums;

namespace HMS.Staff.Domain.Entities
{
    public class Staff
    {
        public Guid Id { get; set; }
        public string StaffNumber { get; set; } = string.Empty;

        // AUTHENTICATION INTEGRATION
        public Guid UserId { get; set; }

        // Employment Information
        public StaffType StaffType { get; set; }
        public string Department { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string Position { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public EmploymentType EmploymentType { get; set; }

        // Professional Information
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseIssueDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public string? Qualifications { get; set; }
        public int YearsOfExperience { get; set; }

        // Work Schedule
        public ShiftType ShiftType { get; set; }
        public string? WorkSchedule { get; set; }
        public int WeeklyWorkHours { get; set; }

        // Compensation
        public decimal BasicSalary { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? TaxId { get; set; }

        // Address
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // Emergency Contact
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }

        // Additional Information
        public string? BloodGroup { get; set; }
        public string? Languages { get; set; }
        public string? Notes { get; set; }

        // System Fields
        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public ICollection<StaffLeave> Leaves { get; set; } = new List<StaffLeave>();
        public ICollection<StaffEducation> EducationRecords { get; set; } = new List<StaffEducation>();
        public ICollection<StaffExperience> ExperienceRecords { get; set; } = new List<StaffExperience>();
        public ICollection<StaffCertification> Certifications { get; set; } = new List<StaffCertification>();
        public ICollection<StaffAttendance> AttendanceRecords { get; set; } = new List<StaffAttendance>();
        public ICollection<StaffPerformanceReview> PerformanceReviews { get; set; } = new List<StaffPerformanceReview>();
    }
}
