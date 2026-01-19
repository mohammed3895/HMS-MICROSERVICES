using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Admin
{
    public class CreateStaffCommand : IRequest<Result<CreateStaffResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string Role { get; set; } = string.Empty; // Doctor, Nurse, Pharmacist, LabTechnician, Receptionist, Admin

        // Staff-specific fields
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? EmployeeId { get; set; }
        public decimal? Salary { get; set; }
        public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
        public string? LicenseNumber { get; set; } // For Doctor, Nurse, Pharmacist
        public DateTime? LicenseExpiryDate { get; set; }
        public string? Qualification { get; set; }
        public int YearsOfExperience { get; set; }

        // Doctor-specific
        public List<string>? Certifications { get; set; }
        public string? ConsultationFee { get; set; }
        public int? MaxPatientsPerDay { get; set; }
        public string? Biography { get; set; }

        // Nurse/Receptionist-specific
        public string? Shift { get; set; }
        public string? Ward { get; set; }

        // Pharmacist-specific
        public string? PharmacyLocation { get; set; }

        // Lab Technician-specific
        public string? LabSection { get; set; }

        // Admin-specific
        public string? Position { get; set; }
        public List<string>? Permissions { get; set; }

        // Created by (admin who created this staff)
        public Guid CreatedBy { get; set; }
    }
}
