namespace HMS.Staff.Application.DTOs
{
    public class CreateStaffWithAuthDto
    {
        // Authentication Data (will be sent to Auth service)
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;

        // Staff-Specific Data
        public string StaffType { get; set; } = string.Empty; // Doctor, Nurse, etc.
        public string Department { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string Position { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public string ShiftType { get; set; } = string.Empty;

        public string? LicenseNumber { get; set; }
        public DateTime? LicenseIssueDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public string? Qualifications { get; set; }
        public int YearsOfExperience { get; set; }

        public decimal BasicSalary { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }
    }
}
