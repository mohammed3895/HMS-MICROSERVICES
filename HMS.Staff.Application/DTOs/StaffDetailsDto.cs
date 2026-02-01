namespace HMS.Staff.Application.DTOs
{
    public class StaffDetailsDto
    {
        public Guid Id { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string NationalId { get; set; } = string.Empty;

        public string StaffType { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string Position { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EmploymentStatus { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string ShiftType { get; set; } = string.Empty;

        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public string? Qualifications { get; set; }
        public int YearsOfExperience { get; set; }

        public decimal BasicSalary { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public List<StaffEducationDto> EducationRecords { get; set; } = new();
        public List<StaffCertificationDto> Certifications { get; set; } = new();
    }
}
