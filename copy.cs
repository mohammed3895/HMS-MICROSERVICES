using HMS.Doctor.Application.Commands;
using HMS.Doctor.Application.DTOs;
using HMS.Doctor.Application.Queries;
using HMS.Doctor.Domain.Entities;
using HMS.Doctor.Domain.Enums;
using HMS.Doctor.Infrastructure.Data;

namespace HMS.Doctor.Domain.Entities
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public string DoctorNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Professional Information
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseIssueDate { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? SubSpecialization { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime JoinDate { get; set; }
        public DoctorStatus Status { get; set; }

        // Consultation Details
        public decimal ConsultationFee { get; set; }
        public int ConsultationDurationMinutes { get; set; } = 30;
        public bool AcceptsInsurance { get; set; }
        public string? InsuranceProviders { get; set; } // Comma-separated list

        // Qualifications
        public string Qualifications { get; set; } = string.Empty; // e.g., "MBBS, MD, FRCS"
        public string? MedicalSchool { get; set; }
        public int? GraduationYear { get; set; }

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
        public string? Languages { get; set; } // Comma-separated list
        public string? Biography { get; set; }
        public string? Awards { get; set; }
        public string? Publications { get; set; }
        public string? ResearchInterests { get; set; }

        // Statistics
        public int TotalPatientsSeen { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Settings
        public bool IsAvailableForTeleconsultation { get; set; }
        public bool IsAcceptingNewPatients { get; set; }
        public int? MaxPatientsPerDay { get; set; }

        // System Fields
        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
        public ICollection<DoctorLeave> Leaves { get; set; } = new List<DoctorLeave>();
        public ICollection<DoctorEducation> EducationRecords { get; set; } = new List<DoctorEducation>();
        public ICollection<DoctorExperience> ExperienceRecords { get; set; } = new List<DoctorExperience>();
        public ICollection<DoctorCertification> Certifications { get; set; } = new List<DoctorCertification>();
        public ICollection<DoctorReview> Reviews { get; set; } = new List<DoctorReview>();
    }

    public class DoctorAvailability
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 30;
        public bool IsAvailable { get; set; } = true;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Doctor Doctor { get; set; } = null!;
    }

    public class DoctorLeave
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }

        // Navigation
        public Doctor Doctor { get; set; } = null!;
    }

    public class DoctorEducation
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Doctor Doctor { get; set; } = null!;
    }

    public class DoctorExperience
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string Organization { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? Department { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentPosition { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Doctor Doctor { get; set; } = null!;
    }

    public class DoctorCertification
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string CertificationName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string? CertificateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool NeverExpires { get; set; }
        public string? DocumentUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Doctor Doctor { get; set; } = null!;
    }

    public class DoctorReview
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid? AppointmentId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }

        // Navigation
        public Doctor Doctor { get; set; } = null!;
    }
}

namespace HMS.Doctor.Domain.Enums
{
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum DoctorStatus
    {
        Active = 1,
        OnLeave = 2,
        Suspended = 3,
        Retired = 4,
        Terminated = 5
    }

    public enum LeaveType
    {
        Annual = 1,
        Sick = 2,
        Emergency = 3,
        Maternity = 4,
        Paternity = 5,
        Study = 6,
        Compensatory = 7,
        Unpaid = 8
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public enum DoctorSearchSortBy
    {
        Name = 1,
        Rating = 2,
        Experience = 3,
        ConsultationFee = 4,
        Availability = 5
    }

    public enum SpecializationCategory
    {
        GeneralPractice = 1,
        Surgery = 2,
        InternalMedicine = 3,
        Pediatrics = 4,
        ObstetricsGynecology = 5,
        Psychiatry = 6,
        Radiology = 7,
        Pathology = 8,
        Anesthesiology = 9,
        EmergencyMedicine = 10,
        Cardiology = 11,
        Dermatology = 12,
        Neurology = 13,
        Orthopedics = 14,
        Ophthalmology = 15,
        ENT = 16,
        Urology = 17,
        Oncology = 18,
        Nephrology = 19,
        Gastroenterology = 20
    }
}

using HMS.Common.DTOs;
using HMS.Doctor.Application.DTOs;
using MediatR;

namespace HMS.Doctor.Application.Commands
{
    // Create Doctor
    public class CreateDoctorCommand : IRequest<Result<Guid>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

        // Professional
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseIssueDate { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? SubSpecialization { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime JoinDate { get; set; }

        // Consultation
        public decimal ConsultationFee { get; set; }
        public int ConsultationDurationMinutes { get; set; } = 30;
        public bool AcceptsInsurance { get; set; }
        public string? InsuranceProviders { get; set; }

        // Qualifications
        public string Qualifications { get; set; } = string.Empty;
        public string? MedicalSchool { get; set; }
        public int? GraduationYear { get; set; }

        // Address
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // Additional
        public string? Languages { get; set; }
        public string? Biography { get; set; }
        public bool IsAvailableForTeleconsultation { get; set; }
        public bool IsAcceptingNewPatients { get; set; } = true;
        public int? MaxPatientsPerDay { get; set; }

        public Guid CreatedBy { get; set; }
    }

    // Update Doctor
    public class UpdateDoctorCommand : IRequest<Result<bool>>
    {
        public Guid DoctorId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhoneNumber { get; set; }

        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? SubSpecialization { get; set; }
        public int YearsOfExperience { get; set; }

        public decimal ConsultationFee { get; set; }
        public int ConsultationDurationMinutes { get; set; }
        public bool AcceptsInsurance { get; set; }
        public string? InsuranceProviders { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public string? Languages { get; set; }
        public string? Biography { get; set; }
        public bool IsAvailableForTeleconsultation { get; set; }
        public bool IsAcceptingNewPatients { get; set; }
        public int? MaxPatientsPerDay { get; set; }

        public Guid UpdatedBy { get; set; }
    }

    // Update Doctor Status
    public class UpdateDoctorStatusCommand : IRequest<Result<bool>>
    {
        public Guid DoctorId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public Guid UpdatedBy { get; set; }
    }

    // Add Doctor Availability
    public class AddDoctorAvailabilityCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public List<AvailabilitySlotDto> Slots { get; set; } = new();
    }

    // Update Doctor Availability
    public class UpdateDoctorAvailabilityCommand : IRequest<Result<bool>>
    {
        public Guid AvailabilityId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
        public bool IsAvailable { get; set; }
    }

    // Apply Leave
    public class ApplyDoctorLeaveCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }

    // Approve Leave
    public class ApproveDoctorLeaveCommand : IRequest<Result<bool>>
    {
        public Guid LeaveId { get; set; }
        public Guid ApprovedBy { get; set; }
    }

    // Reject Leave
    public class RejectDoctorLeaveCommand : IRequest<Result<bool>>
    {
        public Guid LeaveId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public Guid RejectedBy { get; set; }
    }

    // Add Education
    public class AddDoctorEducationCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Country { get; set; }
    }

    // Add Experience
    public class AddDoctorExperienceCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public string Organization { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? Department { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentPosition { get; set; }
        public string? Description { get; set; }
    }

    // Add Certification
    public class AddDoctorCertificationCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public string CertificationName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string? CertificateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool NeverExpires { get; set; }
    }

    // Add Review
    public class AddDoctorReviewCommand : IRequest<Result<Guid>>
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid? AppointmentId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    // Update Profile Picture
    public class UpdateDoctorProfilePictureCommand : IRequest<Result<bool>>
    {
        public Guid DoctorId { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }

    // Delete Doctor (Soft Delete)
    public class DeleteDoctorCommand : IRequest<Result<bool>>
    {
        public Guid DoctorId { get; set; }
        public Guid DeletedBy { get; set; }
    }
}
using HMS.Common.DTOs;
using HMS.Doctor.Application.DTOs;
using MediatR;

namespace HMS.Doctor.Application.Queries
{
    // Get Doctor By ID
    public class GetDoctorByIdQuery : IRequest<Result<DoctorDetailsDto>>
    {
        public Guid DoctorId { get; set; }
    }

    // Get All Doctors with Pagination
    public class GetAllDoctorsQuery : IRequest<Result<PagedResult<DoctorSummaryDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? Status { get; set; }
        public bool? IsAcceptingNewPatients { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }

    // Get Doctors By Department
    public class GetDoctorsByDepartmentQuery : IRequest<Result<List<DoctorSummaryDto>>>
    {
        public string Department { get; set; } = string.Empty;
    }

    // Get Doctors By Specialization
    public class GetDoctorsBySpecializationQuery : IRequest<Result<List<DoctorSummaryDto>>>
    {
        public string Specialization { get; set; } = string.Empty;
    }

    // Search Doctors
    public class SearchDoctorsQuery : IRequest<Result<List<DoctorSearchResultDto>>>
    {
        public string? SearchTerm { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? Language { get; set; }
        public decimal? MaxConsultationFee { get; set; }
        public bool? AcceptsInsurance { get; set; }
        public bool? AvailableForTeleconsultation { get; set; }
        public int? MinRating { get; set; }
        public DayOfWeek? AvailableOnDay { get; set; }
    }

    // Get Doctor Availability
    public class GetDoctorAvailabilityQuery : IRequest<Result<List<DoctorAvailabilityDto>>>
    {
        public Guid DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    // Get Doctor Leaves
    public class GetDoctorLeavesQuery : IRequest<Result<List<DoctorLeaveDto>>>
    {
        public Guid DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
    }

    // Get Doctor Reviews
    public class GetDoctorReviewsQuery : IRequest<Result<PagedResult<DoctorReviewDto>>>
    {
        public Guid DoctorId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool VerifiedOnly { get; set; }
    }

    // Get Doctor Statistics
    public class GetDoctorStatisticsQuery : IRequest<Result<DoctorStatisticsDto>>
    {
        public Guid DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    // Get Doctor Education Records
    public class GetDoctorEducationQuery : IRequest<Result<List<DoctorEducationDto>>>
    {
        public Guid DoctorId { get; set; }
    }

    // Get Doctor Experience Records
    public class GetDoctorExperienceQuery : IRequest<Result<List<DoctorExperienceDto>>>
    {
        public Guid DoctorId { get; set; }
    }

    // Get Doctor Certifications
    public class GetDoctorCertificationsQuery : IRequest<Result<List<DoctorCertificationDto>>>
    {
        public Guid DoctorId { get; set; }
    }

    // Get Available Doctors for Time Slot
    public class GetAvailableDoctorsForSlotQuery : IRequest<Result<List<DoctorSummaryDto>>>
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
    }

    // Get Top Rated Doctors
    public class GetTopRatedDoctorsQuery : IRequest<Result<List<DoctorSummaryDto>>>
    {
        public int Count { get; set; } = 10;
        public string? Department { get; set; }
        public string? Specialization { get; set; }
    }

    // Get Pending Leave Requests
    public class GetPendingLeaveRequestsQuery : IRequest<Result<List<DoctorLeaveDto>>>
    {
        public Guid? DoctorId { get; set; }
    }

    // Get Doctor Dashboard Data
    public class GetDoctorDashboardQuery : IRequest<Result<DoctorDashboardDto>>
    {
        public Guid DoctorId { get; set; }
    }
}

namespace HMS.Doctor.Application.DTOs
{
    public class DoctorDetailsDto
    {
        public Guid Id { get; set; }
        public string DoctorNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }

        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiryDate { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? SubSpecialization { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; }
        public int ConsultationDurationMinutes { get; set; }
        public bool AcceptsInsurance { get; set; }
        public List<string> InsuranceProviders { get; set; } = new();

        public string Qualifications { get; set; } = string.Empty;
        public string? MedicalSchool { get; set; }
        public int? GraduationYear { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public List<string> Languages { get; set; } = new();
        public string? Biography { get; set; }
        public string? Awards { get; set; }
        public string? ResearchInterests { get; set; }

        public int TotalPatientsSeen { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public bool IsAvailableForTeleconsultation { get; set; }
        public bool IsAcceptingNewPatients { get; set; }
        public int? MaxPatientsPerDay { get; set; }

        public List<DoctorAvailabilityDto> Availabilities { get; set; } = new();
        public List<DoctorEducationDto> EducationRecords { get; set; } = new();
        public List<DoctorExperienceDto> ExperienceRecords { get; set; } = new();
        public List<DoctorCertificationDto> Certifications { get; set; } = new();
    }

    public class DoctorSummaryDto
    {
        public Guid Id { get; set; }
        public string DoctorNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? SubSpecialization { get; set; }
        public int YearsOfExperience { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ConsultationFee { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsAcceptingNewPatients { get; set; }
        public bool IsAvailableForTeleconsultation { get; set; }
    }

    public class DoctorSearchResultDto : DoctorSummaryDto
    {
        public List<string> Languages { get; set; } = new();
        public bool AcceptsInsurance { get; set; }
        public List<string> AvailableDays { get; set; } = new();
    }

    public class DoctorAvailabilityDto
    {
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AvailabilitySlotDto
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 30;
    }

    public class DoctorLeaveDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class DoctorEducationDto
    {
        public Guid Id { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Country { get; set; }
    }

    public class DoctorExperienceDto
    {
        public Guid Id { get; set; }
        public string Organization { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? Department { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentPosition { get; set; }
        public string? Description { get; set; }
    }

    public class DoctorCertificationDto
    {
        public Guid Id { get; set; }
        public string CertificationName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string? CertificateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool NeverExpires { get; set; }
        public string? DocumentUrl { get; set; }
    }

    public class DoctorReviewDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
    }

    public class DoctorStatisticsDto
    {
        public Guid DoctorId { get; set; }
        public int TotalPatientsSeen { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> AppointmentsByMonth { get; set; } = new();
        public Dictionary<string, int> AppointmentsByType { get; set; } = new();
    }

    public class DoctorDashboardDto
    {
        public DoctorSummaryDto DoctorInfo { get; set; } = null!;
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointmentsToday { get; set; }
        public List<UpcomingAppointmentDto> NextAppointments { get; set; } = new();
        public int PendingLeaveRequests { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public int ThisMonthPatients { get; set; }
    }

    public class UpcomingAppointmentDto
    {
        public Guid Id { get; set; }
        public string AppointmentNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string ChiefComplaint { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

using HMS.Doctor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HMS.Doctor.Infrastructure.Data
{
    public class DoctorDbContext : DbContext
    {
        public DoctorDbContext(DbContextOptions<DoctorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Entities.Doctor> Doctors { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<DoctorLeave> DoctorLeaves { get; set; }
        public DbSet<DoctorEducation> DoctorEducations { get; set; }
        public DbSet<DoctorExperience> DoctorExperiences { get; set; }
        public DbSet<DoctorCertification> DoctorCertifications { get; set; }
        public DbSet<DoctorReview> DoctorReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Doctor Configuration
            builder.Entity<Domain.Entities.Doctor>(entity =>
            {
                entity.ToTable("Doctors");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.HasIndex(e => e.Department);
                entity.HasIndex(e => e.Specialization);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.IsActive, e.IsDeleted });

                entity.Property(e => e.DoctorNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AlternatePhoneNumber).HasMaxLength(20);
                entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);

                entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Specialization).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SubSpecialization).HasMaxLength(100);

                entity.Property(e => e.ConsultationFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.InsuranceProviders).HasMaxLength(1000);

                entity.Property(e => e.Qualifications).IsRequired().HasMaxLength(500);
                entity.Property(e => e.MedicalSchool).HasMaxLength(200);

                entity.Property(e => e.AddressLine1).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AddressLine2).HasMaxLength(200);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.State).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);

                entity.Property(e => e.EmergencyContactName).HasMaxLength(200);
                entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
                entity.Property(e => e.EmergencyContactRelationship).HasMaxLength(50);

                entity.Property(e => e.Languages).HasMaxLength(500);
                entity.Property(e => e.Biography).HasMaxLength(2000);
                entity.Property(e => e.Awards).HasMaxLength(2000);
                entity.Property(e => e.Publications).HasMaxLength(2000);
                entity.Property(e => e.ResearchInterests).HasMaxLength(1000);

                entity.Property(e => e.AverageRating).HasColumnType("decimal(3,2)");

                entity.HasMany(e => e.Availabilities)
                    .WithOne(e => e.Doctor)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Leaves)
                    .WithOne(e => e.Doctor)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.EducationRecords)
                    .WithOne(e => e.Doctor)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.ExperienceRecords)
                    .WithOne(e => e.Doctor)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Certifications)
                    .WithOne(e => e.Doctor)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Reviews)
                    .WithOne(e => e.Doctor)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // DoctorAvailability Configuration
            builder.Entity<DoctorAvailability>(entity =>
            {
                entity.ToTable("DoctorAvailabilities");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => new { e.DoctorId, e.DayOfWeek, e.IsAvailable });
            });

            // DoctorLeave Configuration
            builder.Entity<DoctorLeave>(entity =>
            {
                entity.ToTable("DoctorLeaves");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.StartDate, e.EndDate });

                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
            });

            // DoctorEducation Configuration
            builder.Entity<DoctorEducation>(entity =>
            {
                entity.ToTable("DoctorEducations");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);

                entity.Property(e => e.Degree).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Institution).IsRequired().HasMaxLength(300);
                entity.Property(e => e.FieldOfStudy).HasMaxLength(200);
                entity.Property(e => e.Country).HasMaxLength(100);
            });

            // DoctorExperience Configuration
            builder.Entity<DoctorExperience>(entity =>
            {
                entity.ToTable("DoctorExperiences");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);

                entity.Property(e => e.Organization).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // DoctorCertification Configuration
            builder.Entity<DoctorCertification>(entity =>
            {
                entity.ToTable("DoctorCertifications");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.ExpiryDate);

                entity.Property(e => e.CertificationName).IsRequired().HasMaxLength(300);
                entity.Property(e => e.IssuingOrganization).IsRequired().HasMaxLength(300);
                entity.Property(e => e.CertificateNumber).HasMaxLength(100);
                entity.Property(e => e.DocumentUrl).HasMaxLength(500);
            });

            // DoctorReview Configuration
            builder.Entity<DoctorReview>(entity =>
            {
                entity.ToTable("DoctorReviews");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.AppointmentId);
                entity.HasIndex(e => e.Rating);
                entity.HasIndex(e => new { e.DoctorId, e.IsVerified });

                entity.Property(e => e.PatientName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Comment).HasMaxLength(1000);
            });
        }
    }

    // DbContext Factory for Migrations
    public class DoctorDbContextFactory : IDesignTimeDbContextFactory<DoctorDbContext>
    {
        public DoctorDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HMS_MS_DOCTOR;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

            var optionsBuilder = new DbContextOptionsBuilder<DoctorDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("HMS.Doctor.Infrastructure"));

            return new DoctorDbContext(optionsBuilder.Options);
        }
    }
}

using HMS.Doctor.Application.Commands;
using HMS.Doctor.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Doctor.API.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(
            IMediator mediator,
            ILogger<DoctorController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all doctors with pagination and filters
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDoctors(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? department = null,
            [FromQuery] string? specialization = null,
            [FromQuery] string? status = null,
            [FromQuery] bool? isAcceptingNewPatients = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            var query = new GetAllDoctorsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Department = department,
                Specialization = specialization,
                Status = status,
                IsAcceptingNewPatients = isAcceptingNewPatients,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor by ID
        /// </summary>
        [HttpGet("{doctorId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorById(Guid doctorId)
        {
            var query = new GetDoctorByIdQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Create a new doctor profile
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorCommand command)
        {
            command.CreatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update doctor profile
        /// </summary>
        [HttpPut("{doctorId}")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDoctor(
            Guid doctorId,
            [FromBody] UpdateDoctorCommand command)
        {
            command.DoctorId = doctorId;
            command.UpdatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update doctor status
        /// </summary>
        [HttpPut("{doctorId}/status")]
        [Authorize(Roles = "Admin,HR")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDoctorStatus(
            Guid doctorId,
            [FromBody] UpdateDoctorStatusCommand command)
        {
            command.DoctorId = doctorId;
            command.UpdatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete doctor (soft delete)
        /// </summary>
        [HttpDelete("{doctorId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDoctor(Guid doctorId)
        {
            var command = new DeleteDoctorCommand
            {
                DoctorId = doctorId,
                DeletedBy = GetCurrentUserId()
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search doctors with advanced filters
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchDoctors(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? department = null,
            [FromQuery] string? specialization = null,
            [FromQuery] string? language = null,
            [FromQuery] decimal? maxConsultationFee = null,
            [FromQuery] bool? acceptsInsurance = null,
            [FromQuery] bool? availableForTeleconsultation = null,
            [FromQuery] int? minRating = null,
            [FromQuery] DayOfWeek? availableOnDay = null)
        {
            var query = new SearchDoctorsQuery
            {
                SearchTerm = searchTerm,
                Department = department,
                Specialization = specialization,
                Language = language,
                MaxConsultationFee = maxConsultationFee,
                AcceptsInsurance = acceptsInsurance,
                AvailableForTeleconsultation = availableForTeleconsultation,
                MinRating = minRating,
                AvailableOnDay = availableOnDay
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctors by department
        /// </summary>
        [HttpGet("department/{department}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorsByDepartment(string department)
        {
            var query = new GetDoctorsByDepartmentQuery { Department = department };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctors by specialization
        /// </summary>
        [HttpGet("specialization/{specialization}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
        {
            var query = new GetDoctorsBySpecializationQuery { Specialization = specialization };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get top rated doctors
        /// </summary>
        [HttpGet("top-rated")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopRatedDoctors(
            [FromQuery] int count = 10,
            [FromQuery] string? department = null,
            [FromQuery] string? specialization = null)
        {
            var query = new GetTopRatedDoctorsQuery
            {
                Count = count,
                Department = department,
                Specialization = specialization
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get available doctors for specific time slot
        /// </summary>
        [HttpGet("available")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableDoctors(
            [FromQuery] DateTime date,
            [FromQuery] TimeSpan startTime,
            [FromQuery] TimeSpan endTime,
            [FromQuery] string? department = null,
            [FromQuery] string? specialization = null)
        {
            var query = new GetAvailableDoctorsForSlotQuery
            {
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                Department = department,
                Specialization = specialization
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor availability
        /// </summary>
        [HttpGet("{doctorId}/availability")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorAvailability(
            Guid doctorId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetDoctorAvailabilityQuery
            {
                DoctorId = doctorId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add doctor availability schedule
        /// </summary>
        [HttpPost("{doctorId}/availability")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddDoctorAvailability(
            Guid doctorId,
            [FromBody] AddDoctorAvailabilityCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update doctor availability
        /// </summary>
        [HttpPut("{doctorId}/availability/{availabilityId}")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDoctorAvailability(
            Guid availabilityId,
            [FromBody] UpdateDoctorAvailabilityCommand command)
        {
            command.AvailabilityId = availabilityId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor leaves
        /// </summary>
        [HttpGet("{doctorId}/leaves")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorLeaves(
            Guid doctorId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? status = null)
        {
            var query = new GetDoctorLeavesQuery
            {
                DoctorId = doctorId,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Apply for leave
        /// </summary>
        [HttpPost("{doctorId}/leaves")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApplyLeave(
            Guid doctorId,
            [FromBody] ApplyDoctorLeaveCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Approve leave request
        /// </summary>
        [HttpPost("leaves/{leaveId}/approve")]
        [Authorize(Roles = "Admin,HR")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveLeave(Guid leaveId)
        {
            var command = new ApproveDoctorLeaveCommand
            {
                LeaveId = leaveId,
                ApprovedBy = GetCurrentUserId()
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reject leave request
        /// </summary>
        [HttpPost("leaves/{leaveId}/reject")]
        [Authorize(Roles = "Admin,HR")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectLeave(
            Guid leaveId,
            [FromBody] RejectDoctorLeaveCommand command)
        {
            command.LeaveId = leaveId;
            command.RejectedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get pending leave requests
        /// </summary>
        [HttpGet("leaves/pending")]
        [Authorize(Roles = "Admin,HR")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingLeaveRequests([FromQuery] Guid? doctorId = null)
        {
            var query = new GetPendingLeaveRequestsQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor education records
        /// </summary>
        [HttpGet("{doctorId}/education")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorEducation(Guid doctorId)
        {
            var query = new GetDoctorEducationQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add doctor education record
        /// </summary>
        [HttpPost("{doctorId}/education")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddDoctorEducation(
            Guid doctorId,
            [FromBody] AddDoctorEducationCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor experience records
        /// </summary>
        [HttpGet("{doctorId}/experience")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorExperience(Guid doctorId)
        {
            var query = new GetDoctorExperienceQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add doctor experience record
        /// </summary>
        [HttpPost("{doctorId}/experience")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddDoctorExperience(
            Guid doctorId,
            [FromBody] AddDoctorExperienceCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor certifications
        /// </summary>
        [HttpGet("{doctorId}/certifications")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorCertifications(Guid doctorId)
        {
            var query = new GetDoctorCertificationsQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add doctor certification
        /// </summary>
        [HttpPost("{doctorId}/certifications")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddDoctorCertification(
            Guid doctorId,
            [FromBody] AddDoctorCertificationCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor reviews
        /// </summary>
        [HttpGet("{doctorId}/reviews")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorReviews(
            Guid doctorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool verifiedOnly = false)
        {
            var query = new GetDoctorReviewsQuery
            {
                DoctorId = doctorId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                VerifiedOnly = verifiedOnly
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add doctor review
        /// </summary>
        [HttpPost("{doctorId}/reviews")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddDoctorReview(
            Guid doctorId,
            [FromBody] AddDoctorReviewCommand command)
        {
            command.DoctorId = doctorId;
            command.PatientId = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor statistics
        /// </summary>
        [HttpGet("{doctorId}/statistics")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorStatistics(
            Guid doctorId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetDoctorStatisticsQuery
            {
                DoctorId = doctorId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor dashboard data
        /// </summary>
        [HttpGet("{doctorId}/dashboard")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorDashboard(Guid doctorId)
        {
            var query = new GetDoctorDashboardQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update profile picture
        /// </summary>
        [HttpPut("{doctorId}/profile-picture")]
        [Authorize(Roles = "Admin,HR,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfilePicture(
            Guid doctorId,
            [FromBody] UpdateDoctorProfilePictureCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // ==================== Helper Methods ====================

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("user_id")?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return userId;
        }
    }
}

using HMS.Common.DTOs;
using HMS.Doctor.Application.Commands;
using HMS.Doctor.Domain.Enums;
using HMS.Doctor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Doctor.Application.Handlers
{
    public class CreateDoctorCommandHandler
        : IRequestHandler<CreateDoctorCommand, Result<Guid>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<CreateDoctorCommandHandler> _logger;

        public CreateDoctorCommandHandler(
            DoctorDbContext context,
            ILogger<CreateDoctorCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(
            CreateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate email uniqueness
                if (await _context.Doctors.AnyAsync(d => d.Email == request.Email, cancellationToken))
                {
                    return Result<Guid>.Failure("A doctor with this email already exists");
                }

                // Validate license number uniqueness
                if (await _context.Doctors.AnyAsync(d => d.LicenseNumber == request.LicenseNumber, cancellationToken))
                {
                    return Result<Guid>.Failure("A doctor with this license number already exists");
                }

                // Generate doctor number
                var doctorCount = await _context.Doctors.CountAsync(cancellationToken);
                var doctorNumber = $"DOC-{DateTime.UtcNow.Year}-{(doctorCount + 1):D6}";

                // Create doctor entity
                var doctor = new Domain.Entities.Doctor
                {
                    Id = Guid.NewGuid(),
                    DoctorNumber = doctorNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    AlternatePhoneNumber = request.AlternatePhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = Enum.Parse<Gender>(request.Gender),

                    LicenseNumber = request.LicenseNumber,
                    LicenseIssueDate = request.LicenseIssueDate,
                    LicenseExpiryDate = request.LicenseExpiryDate,
                    Department = request.Department,
                    Specialization = request.Specialization,
                    SubSpecialization = request.SubSpecialization,
                    YearsOfExperience = request.YearsOfExperience,
                    JoinDate = request.JoinDate,
                    Status = DoctorStatus.Active,

                    ConsultationFee = request.ConsultationFee,
                    ConsultationDurationMinutes = request.ConsultationDurationMinutes,
                    AcceptsInsurance = request.AcceptsInsurance,
                    InsuranceProviders = request.InsuranceProviders,

                    Qualifications = request.Qualifications,
                    MedicalSchool = request.MedicalSchool,
                    GraduationYear = request.GraduationYear,

                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    PostalCode = request.PostalCode,

                    Languages = request.Languages,
                    Biography = request.Biography,
                    IsAvailableForTeleconsultation = request.IsAvailableForTeleconsultation,
                    IsAcceptingNewPatients = request.IsAcceptingNewPatients,
                    MaxPatientsPerDay = request.MaxPatientsPerDay,

                    IsActive = true,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Doctor created successfully: {DoctorNumber} - {FullName}",
                    doctor.DoctorNumber, doctor.FullName);

                return Result<Guid>.Success(doctor.Id, "Doctor created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return Result<Guid>.Failure("An error occurred while creating the doctor");
            }
        }
    }
}

using HMS.Common.DTOs;
using HMS.Doctor.Application.DTOs;
using HMS.Doctor.Application.Queries;
using HMS.Doctor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Doctor.Application.Handlers
{
    public class GetDoctorByIdQueryHandler
        : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDetailsDto>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<GetDoctorByIdQueryHandler> _logger;

        public GetDoctorByIdQueryHandler(
            DoctorDbContext context,
            ILogger<GetDoctorByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<DoctorDetailsDto>> Handle(
            GetDoctorByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Availabilities)
                    .Include(d => d.EducationRecords)
                    .Include(d => d.ExperienceRecords)
                    .Include(d => d.Certifications)
                    .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

                if (doctor == null)
                {
                    return Result<DoctorDetailsDto>.Failure("Doctor not found");
                }

                var dto = new DoctorDetailsDto
                {
                    Id = doctor.Id,
                    DoctorNumber = doctor.DoctorNumber,
                    FullName = doctor.FullName,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Email = doctor.Email,
                    PhoneNumber = doctor.PhoneNumber,
                    AlternatePhoneNumber = doctor.AlternatePhoneNumber,
                    DateOfBirth = doctor.DateOfBirth,
                    Gender = doctor.Gender.ToString(),
                    ProfilePictureUrl = doctor.ProfilePictureUrl,

                    LicenseNumber = doctor.LicenseNumber,
                    LicenseExpiryDate = doctor.LicenseExpiryDate,
                    Department = doctor.Department,
                    Specialization = doctor.Specialization,
                    SubSpecialization = doctor.SubSpecialization,
                    YearsOfExperience = doctor.YearsOfExperience,
                    JoinDate = doctor.JoinDate,
                    Status = doctor.Status.ToString(),

                    ConsultationFee = doctor.ConsultationFee,
                    ConsultationDurationMinutes = doctor.ConsultationDurationMinutes,
                    AcceptsInsurance = doctor.AcceptsInsurance,
                    InsuranceProviders = string.IsNullOrEmpty(doctor.InsuranceProviders)
                        ? new List<string>()
                        : doctor.InsuranceProviders.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),

                    Qualifications = doctor.Qualifications,
                    MedicalSchool = doctor.MedicalSchool,
                    GraduationYear = doctor.GraduationYear,

                    AddressLine1 = doctor.AddressLine1,
                    AddressLine2 = doctor.AddressLine2,
                    City = doctor.City,
                    State = doctor.State,
                    Country = doctor.Country,
                    PostalCode = doctor.PostalCode,

                    Languages = string.IsNullOrEmpty(doctor.Languages)
                        ? new List<string>()
                        : doctor.Languages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                    Biography = doctor.Biography,
                    Awards = doctor.Awards,
                    ResearchInterests = doctor.ResearchInterests,

                    TotalPatientsSeen = doctor.TotalPatientsSeen,
                    AverageRating = doctor.AverageRating,
                    TotalReviews = doctor.TotalReviews,

                    IsAvailableForTeleconsultation = doctor.IsAvailableForTeleconsultation,
                    IsAcceptingNewPatients = doctor.IsAcceptingNewPatients,
                    MaxPatientsPerDay = doctor.MaxPatientsPerDay,

                    Availabilities = doctor.Availabilities.Select(a => new DoctorAvailabilityDto
                    {
                        Id = a.Id,
                        DayOfWeek = a.DayOfWeek.ToString(),
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        SlotDurationMinutes = a.SlotDurationMinutes,
                        IsAvailable = a.IsAvailable
                    }).ToList(),

                    EducationRecords = doctor.EducationRecords.Select(e => new DoctorEducationDto
                    {
                        Id = e.Id,
                        Degree = e.Degree,
                        Institution = e.Institution,
                        FieldOfStudy = e.FieldOfStudy,
                        StartYear = e.StartYear,
                        EndYear = e.EndYear,
                        Country = e.Country
                    }).ToList(),

                    ExperienceRecords = doctor.ExperienceRecords.Select(e => new DoctorExperienceDto
                    {
                        Id = e.Id,
                        Organization = e.Organization,
                        Position = e.Position,
                        Department = e.Department,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        IsCurrentPosition = e.IsCurrentPosition,
                        Description = e.Description
                    }).ToList(),

                    Certifications = doctor.Certifications.Select(c => new DoctorCertificationDto
                    {
                        Id = c.Id,
                        CertificationName = c.CertificationName,
                        IssuingOrganization = c.IssuingOrganization,
                        CertificateNumber = c.CertificateNumber,
                        IssueDate = c.IssueDate,
                        ExpiryDate = c.ExpiryDate,
                        NeverExpires = c.NeverExpires,
                        DocumentUrl = c.DocumentUrl
                    }).ToList()
                };

                _logger.LogInformation("Retrieved doctor details for {DoctorNumber}", doctor.DoctorNumber);

                return Result<DoctorDetailsDto>.Success(dto, "Doctor retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor");
                return Result<DoctorDetailsDto>.Failure("An error occurred while retrieving the doctor");
            }
        }
    }
}

using HMS.Doctor.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/doctor-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HMS Doctor Service API",
        Version = "v1",
        Description = "Hospital Management System - Doctor Service API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<DoctorDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("HMS.Doctor.Infrastructure")));

// MediatR Configuration
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("HMS.Doctor.Application"));
});

// HttpClient Configuration
builder.Services.AddHttpClient("AppointmentService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AppointmentService"] ?? "http://localhost:5003");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DoctorDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS Doctor Service API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DoctorDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");

        // Seed data
        // await DoctorDbSeeder.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
    }
}

Log.Information("HMS Doctor Service starting...");

app.Run();

Log.Information("HMS Doctor Service stopped");

{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
        }
    },
  "AllowedHosts": "*",
  "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HMS_MS_DOCTOR;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "JwtSettings": {
        "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration123!@#",
    "Issuer": "HMS-DoctorService",
    "Audience": "HMS-Clients",
    "ExpiryMinutes": 60
  },
  "Services": {
        "AppointmentService": "http://localhost:5003",
    "PatientService": "http://localhost:5002",
    "NotificationService": "http://localhost:5010"
  },
  "Serilog": {
        "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
            "Default": "Information",
      "Override": {
                "Microsoft": "Warning",
        "System": "Warning"
      }
        },
    "WriteTo": [
      {
            "Name": "Console",
        "Args": {
                "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
        },
      {
            "Name": "File",
        "Args": {
                "path": "Logs/doctor-service-.log",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
        }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}

using HMS.Common.DTOs;
using HMS.Doctor.Application.Commands;
using HMS.Doctor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Doctor.Application.Handlers
{
    public class UpdateDoctorCommandHandler
        : IRequestHandler<UpdateDoctorCommand, Result<bool>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<UpdateDoctorCommandHandler> _logger;

        public UpdateDoctorCommandHandler(
            DoctorDbContext context,
            ILogger<UpdateDoctorCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            UpdateDoctorCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

                if (doctor == null)
                {
                    return Result<bool>.Failure("Doctor not found");
                }

                // Update fields
                doctor.FirstName = request.FirstName;
                doctor.LastName = request.LastName;
                doctor.PhoneNumber = request.PhoneNumber;
                doctor.AlternatePhoneNumber = request.AlternatePhoneNumber;

                doctor.Department = request.Department;
                doctor.Specialization = request.Specialization;
                doctor.SubSpecialization = request.SubSpecialization;
                doctor.YearsOfExperience = request.YearsOfExperience;

                doctor.ConsultationFee = request.ConsultationFee;
                doctor.ConsultationDurationMinutes = request.ConsultationDurationMinutes;
                doctor.AcceptsInsurance = request.AcceptsInsurance;
                doctor.InsuranceProviders = request.InsuranceProviders;

                doctor.AddressLine1 = request.AddressLine1;
                doctor.AddressLine2 = request.AddressLine2;
                doctor.City = request.City;
                doctor.State = request.State;
                doctor.PostalCode = request.PostalCode;

                doctor.Languages = request.Languages;
                doctor.Biography = request.Biography;
                doctor.IsAvailableForTeleconsultation = request.IsAvailableForTeleconsultation;
                doctor.IsAcceptingNewPatients = request.IsAcceptingNewPatients;
                doctor.MaxPatientsPerDay = request.MaxPatientsPerDay;

                doctor.UpdatedBy = request.UpdatedBy;
                doctor.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Doctor updated successfully: {DoctorNumber}",
                    doctor.DoctorNumber);

                return Result<bool>.Success(true, "Doctor updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor: {DoctorId}", request.DoctorId);
                return Result<bool>.Failure("An error occurred while updating the doctor");
            }
        }
    }
}

using HMS.Common.DTOs;
using HMS.Doctor.Application.DTOs;
using HMS.Doctor.Application.Queries;
using HMS.Doctor.Domain.Enums;
using HMS.Doctor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Doctor.Application.Handlers
{
    public class GetAllDoctorsQueryHandler
        : IRequestHandler<GetAllDoctorsQuery, Result<PagedResult<DoctorSummaryDto>>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<GetAllDoctorsQueryHandler> _logger;

        public GetAllDoctorsQueryHandler(
            DoctorDbContext context,
            ILogger<GetAllDoctorsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<PagedResult<DoctorSummaryDto>>> Handle(
            GetAllDoctorsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Doctors.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var search = request.SearchTerm.ToLower();
                    query = query.Where(d =>
                        d.FirstName.ToLower().Contains(search) ||
                        d.LastName.ToLower().Contains(search) ||
                        d.DoctorNumber.ToLower().Contains(search) ||
                        d.Email.ToLower().Contains(search) ||
                        d.Specialization.ToLower().Contains(search));
                }

                if (!string.IsNullOrWhiteSpace(request.Department))
                {
                    query = query.Where(d => d.Department == request.Department);
                }

                if (!string.IsNullOrWhiteSpace(request.Specialization))
                {
                    query = query.Where(d => d.Specialization == request.Specialization);
                }

                if (!string.IsNullOrWhiteSpace(request.Status) &&
                    Enum.TryParse<DoctorStatus>(request.Status, out var status))
                {
                    query = query.Where(d => d.Status == status);
                }

                if (request.IsAcceptingNewPatients.HasValue)
                {
                    query = query.Where(d => d.IsAcceptingNewPatients == request.IsAcceptingNewPatients.Value);
                }

                // Get total count
                var totalCount = await query.CountAsync(cancellationToken);

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "name" => request.SortDescending
                        ? query.OrderByDescending(d => d.LastName).ThenByDescending(d => d.FirstName)
                        : query.OrderBy(d => d.LastName).ThenBy(d => d.FirstName),
                    "rating" => request.SortDescending
                        ? query.OrderByDescending(d => d.AverageRating)
                        : query.OrderBy(d => d.AverageRating),
                    "experience" => request.SortDescending
                        ? query.OrderByDescending(d => d.YearsOfExperience)
                        : query.OrderBy(d => d.YearsOfExperience),
                    "fee" => request.SortDescending
                        ? query.OrderByDescending(d => d.ConsultationFee)
                        : query.OrderBy(d => d.ConsultationFee),
                    _ => query.OrderBy(d => d.LastName).ThenBy(d => d.FirstName)
                };

                // Apply pagination
                var doctors = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(d => new DoctorSummaryDto
                    {
                        Id = d.Id,
                        DoctorNumber = d.DoctorNumber,
                        FullName = d.FullName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        ProfilePictureUrl = d.ProfilePictureUrl,
                        Department = d.Department,
                        Specialization = d.Specialization,
                        SubSpecialization = d.SubSpecialization,
                        YearsOfExperience = d.YearsOfExperience,
                        Status = d.Status.ToString(),
                        ConsultationFee = d.ConsultationFee,
                        AverageRating = d.AverageRating,
                        TotalReviews = d.TotalReviews,
                        IsAcceptingNewPatients = d.IsAcceptingNewPatients,
                        IsAvailableForTeleconsultation = d.IsAvailableForTeleconsultation
                    })
                    .ToListAsync(cancellationToken);

                var result = new PagedResult<DoctorSummaryDto>
                {
                    Items = doctors,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                _logger.LogInformation(
                    "Retrieved {Count} doctors (page {Page} of {TotalPages})",
                    doctors.Count, request.PageNumber, result.TotalPages);

                return Result<PagedResult<DoctorSummaryDto>>.Success(
                    result,
                    "Doctors retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors");
                return Result<PagedResult<DoctorSummaryDto>>.Failure(
                    "An error occurred while retrieving doctors");
            }
        }
    }
}

using HMS.Common.DTOs;
using HMS.Doctor.Application.DTOs;
using HMS.Doctor.Application.Queries;
using HMS.Doctor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Doctor.Application.Handlers
{
    public class SearchDoctorsQueryHandler
        : IRequestHandler<SearchDoctorsQuery, Result<List<DoctorSearchResultDto>>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<SearchDoctorsQueryHandler> _logger;

        public SearchDoctorsQueryHandler(
            DoctorDbContext context,
            ILogger<SearchDoctorsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<DoctorSearchResultDto>>> Handle(
            SearchDoctorsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Doctors
                    .Include(d => d.Availabilities)
                    .Where(d => d.IsActive && !d.IsDeleted)
                    .AsQueryable();

                // Apply search term
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var search = request.SearchTerm.ToLower();
                    query = query.Where(d =>
                        d.FirstName.ToLower().Contains(search) ||
                        d.LastName.ToLower().Contains(search) ||
                        d.Specialization.ToLower().Contains(search) ||
                        d.SubSpecialization != null && d.SubSpecialization.ToLower().Contains(search));
                }

                // Apply filters
                if (!string.IsNullOrWhiteSpace(request.Department))
                {
                    query = query.Where(d => d.Department == request.Department);
                }

                if (!string.IsNullOrWhiteSpace(request.Specialization))
                {
                    query = query.Where(d => d.Specialization == request.Specialization);
                }

                if (!string.IsNullOrWhiteSpace(request.Language))
                {
                    query = query.Where(d => d.Languages != null && d.Languages.Contains(request.Language));
                }

                if (request.MaxConsultationFee.HasValue)
                {
                    query = query.Where(d => d.ConsultationFee <= request.MaxConsultationFee.Value);
                }

                if (request.AcceptsInsurance.HasValue)
                {
                    query = query.Where(d => d.AcceptsInsurance == request.AcceptsInsurance.Value);
                }

                if (request.AvailableForTeleconsultation.HasValue)
                {
                    query = query.Where(d => d.IsAvailableForTeleconsultation == request.AvailableForTeleconsultation.Value);
                }

                if (request.MinRating.HasValue)
                {
                    query = query.Where(d => d.AverageRating >= request.MinRating.Value);
                }

                if (request.AvailableOnDay.HasValue)
                {
                    query = query.Where(d => d.Availabilities
                        .Any(a => a.DayOfWeek == request.AvailableOnDay.Value && a.IsAvailable));
                }

                // Execute query
                var doctors = await query
                    .OrderByDescending(d => d.AverageRating)
                    .ThenBy(d => d.ConsultationFee)
                    .Take(50) // Limit results
                    .Select(d => new DoctorSearchResultDto
                    {
                        Id = d.Id,
                        DoctorNumber = d.DoctorNumber,
                        FullName = d.FullName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        ProfilePictureUrl = d.ProfilePictureUrl,
                        Department = d.Department,
                        Specialization = d.Specialization,
                        SubSpecialization = d.SubSpecialization,
                        YearsOfExperience = d.YearsOfExperience,
                        Status = d.Status.ToString(),
                        ConsultationFee = d.ConsultationFee,
                        AverageRating = d.AverageRating,
                        TotalReviews = d.TotalReviews,
                        IsAcceptingNewPatients = d.IsAcceptingNewPatients,
                        IsAvailableForTeleconsultation = d.IsAvailableForTeleconsultation,
                        Languages = string.IsNullOrEmpty(d.Languages)
                            ? new List<string>()
                            : d.Languages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                        AcceptsInsurance = d.AcceptsInsurance,
                        AvailableDays = d.Availabilities
                            .Where(a => a.IsAvailable)
                            .Select(a => a.DayOfWeek.ToString())
                            .Distinct()
                            .ToList()
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation(
                    "Search returned {Count} doctors", doctors.Count);

                return Result<List<DoctorSearchResultDto>>.Success(
                    doctors,
                    "Search completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors");
                return Result<List<DoctorSearchResultDto>>.Failure(
                    "An error occurred while searching for doctors");
            }
        }
    }
}

using HMS.Common.DTOs;
using HMS.Doctor.Application.Commands;
using HMS.Doctor.Domain.Enums;
using HMS.Doctor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Doctor.Application.Handlers
{
    // Apply Leave Handler
    public class ApplyDoctorLeaveCommandHandler
        : IRequestHandler<ApplyDoctorLeaveCommand, Result<Guid>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<ApplyDoctorLeaveCommandHandler> _logger;

        public ApplyDoctorLeaveCommandHandler(
            DoctorDbContext context,
            ILogger<ApplyDoctorLeaveCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(
            ApplyDoctorLeaveCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

                if (doctor == null)
                {
                    return Result<Guid>.Failure("Doctor not found");
                }

                // Check for overlapping leave
                var hasOverlap = await _context.DoctorLeaves
                    .AnyAsync(l =>
                        l.DoctorId == request.DoctorId &&
                        l.Status != LeaveStatus.Rejected &&
                        l.Status != LeaveStatus.Cancelled &&
                        ((l.StartDate <= request.EndDate && l.EndDate >= request.StartDate)),
                        cancellationToken);

                if (hasOverlap)
                {
                    return Result<Guid>.Failure("You already have leave scheduled for this period");
                }

                var leave = new Domain.Entities.DoctorLeave
                {
                    Id = Guid.NewGuid(),
                    DoctorId = request.DoctorId,
                    Type = Enum.Parse<LeaveType>(request.LeaveType),
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Reason = request.Reason,
                    Status = LeaveStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.DoctorLeaves.Add(leave);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Leave applied for doctor {DoctorId} from {StartDate} to {EndDate}",
                    request.DoctorId, request.StartDate, request.EndDate);

                return Result<Guid>.Success(leave.Id, "Leave application submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying leave for doctor");
                return Result<Guid>.Failure("An error occurred while applying for leave");
            }
        }
    }

    // Approve Leave Handler
    public class ApproveDoctorLeaveCommandHandler
        : IRequestHandler<ApproveDoctorLeaveCommand, Result<bool>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<ApproveDoctorLeaveCommandHandler> _logger;

        public ApproveDoctorLeaveCommandHandler(
            DoctorDbContext context,
            ILogger<ApproveDoctorLeaveCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            ApproveDoctorLeaveCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var leave = await _context.DoctorLeaves
                    .FirstOrDefaultAsync(l => l.Id == request.LeaveId, cancellationToken);

                if (leave == null)
                {
                    return Result<bool>.Failure("Leave request not found");
                }

                if (leave.Status != LeaveStatus.Pending)
                {
                    return Result<bool>.Failure("Only pending leave requests can be approved");
                }

                leave.Status = LeaveStatus.Approved;
                leave.ApprovedBy = request.ApprovedBy;
                leave.ApprovedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Leave approved for doctor {DoctorId}", leave.DoctorId);

                return Result<bool>.Success(true, "Leave approved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave");
                return Result<bool>.Failure("An error occurred while approving leave");
            }
        }
    }

    // Reject Leave Handler
    public class RejectDoctorLeaveCommandHandler
        : IRequestHandler<RejectDoctorLeaveCommand, Result<bool>>
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<RejectDoctorLeaveCommandHandler> _logger;

        public RejectDoctorLeaveCommandHandler(
            DoctorDbContext context,
            ILogger<RejectDoctorLeaveCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            RejectDoctorLeaveCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var leave = await _context.DoctorLeaves
                    .FirstOrDefaultAsync(l => l.Id == request.LeaveId, cancellationToken);

                if (leave == null)
                {
                    return Result<bool>.Failure("Leave request not found");
                }

                if (leave.Status != LeaveStatus.Pending)
                {
                    return Result<bool>.Failure("Only pending leave requests can be rejected");
                }

                leave.Status = LeaveStatus.Rejected;
                leave.RejectedBy = request.RejectedBy;
                leave.RejectedAt = DateTime.UtcNow;
                leave.RejectionReason = request.RejectionReason;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Leave rejected for doctor {DoctorId}", leave.DoctorId);

                return Result<bool>.Success(true, "Leave request rejected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave");
                return Result<bool>.Failure("An error occurred while rejecting leave");
            }
        }
    }
}

using HMS.Doctor.Domain.Entities;
using HMS.Doctor.Domain.Enums;
using HMS.Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Doctor.Infrastructure.Data
{
    public static class DoctorDbSeeder
    {
        public static async Task SeedAsync(DoctorDbContext context, ILogger logger)
        {
            try
            {
                if (await context.Doctors.AnyAsync())
                {
                    logger.LogInformation("Database already seeded");
                    return;
                }

                logger.LogInformation("Starting to seed doctor database...");

                var doctors = new List<Domain.Entities.Doctor>
                {
                    new Domain.Entities.Doctor
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        DoctorNumber = "DOC-2025-000001",
                        FirstName = "Sarah",
                        LastName = "Johnson",
                        Email = "sarah.johnson@hospital.com",
                        PhoneNumber = "+1234567890",
                        DateOfBirth = new DateTime(1980, 5, 15),
                        Gender = Gender.Female,
                        LicenseNumber = "MD-2015-12345",
                        LicenseIssueDate = new DateTime(2015, 6, 1),
                        LicenseExpiryDate = new DateTime(2030, 6, 1),
                        Department = "Cardiology",
                        Specialization = "Cardiologist",
                        SubSpecialization = "Interventional Cardiology",
                        YearsOfExperience = 15,
                        JoinDate = new DateTime(2020, 1, 1),
                        Status = DoctorStatus.Active,
                        ConsultationFee = 200.00m,
                        ConsultationDurationMinutes = 30,
                        AcceptsInsurance = true,
                        InsuranceProviders = "Blue Cross,Aetna,United Healthcare",
                        Qualifications = "MBBS, MD (Cardiology), FACC",
                        MedicalSchool = "Harvard Medical School",
                        GraduationYear = 2005,
                        AddressLine1 = "123 Medical Center Dr",
                        City = "New York",
                        State = "NY",
                        Country = "USA",
                        PostalCode = "10001",
                        Languages = "English,Spanish",
                        Biography = "Dr. Johnson is a board-certified cardiologist with over 15 years of experience.",
                        IsAvailableForTeleconsultation = true,
                        IsAcceptingNewPatients = true,
                        MaxPatientsPerDay = 20,
                        TotalPatientsSeen = 2500,
                        AverageRating = 4.8m,
                        TotalReviews = 150,
                        IsActive = true,
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Domain.Entities.Doctor
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        DoctorNumber = "DOC-2025-000002",
                        FirstName = "Michael",
                        LastName = "Chen",
                        Email = "michael.chen@hospital.com",
                        PhoneNumber = "+1234567891",
                        DateOfBirth = new DateTime(1985, 8, 20),
                        Gender = Gender.Male,
                        LicenseNumber = "MD-2018-23456",
                        LicenseIssueDate = new DateTime(2018, 7, 1),
                        LicenseExpiryDate = new DateTime(2033, 7, 1),
                        Department = "Pediatrics",
                        Specialization = "Pediatrician",
                        YearsOfExperience = 10,
                        JoinDate = new DateTime(2021, 3, 15),
                        Status = DoctorStatus.Active,
                        ConsultationFee = 150.00m,
                        ConsultationDurationMinutes = 20,
                        AcceptsInsurance = true,
                        InsuranceProviders = "Cigna,Blue Cross,Kaiser",
                        Qualifications = "MBBS, MD (Pediatrics), FAAP",
                        MedicalSchool = "Johns Hopkins University",
                        GraduationYear = 2010,
                        AddressLine1 = "456 Children Hospital Rd",
                        City = "Los Angeles",
                        State = "CA",
                        Country = "USA",
                        PostalCode = "90001",
                        Languages = "English,Mandarin",
                        Biography = "Experienced pediatrician specializing in child development and preventive care.",
                        IsAvailableForTeleconsultation = true,
                        IsAcceptingNewPatients = true,
                        MaxPatientsPerDay = 30,
                        TotalPatientsSeen = 3200,
                        AverageRating = 4.9m,
                        TotalReviews = 200,
                        IsActive = true,
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Domain.Entities.Doctor
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        DoctorNumber = "DOC-2025-000003",
                        FirstName = "Emily",
                        LastName = "Rodriguez",
                        Email = "emily.rodriguez@hospital.com",
                        PhoneNumber = "+1234567892",
                        DateOfBirth = new DateTime(1982, 3, 10),
                        Gender = Gender.Female,
                        LicenseNumber = "MD-2016-34567",
                        LicenseIssueDate = new DateTime(2016, 5, 1),
                        LicenseExpiryDate = new DateTime(2031, 5, 1),
                        Department = "Orthopedics",
                        Specialization = "Orthopedic Surgeon",
                        SubSpecialization = "Sports Medicine",
                        YearsOfExperience = 12,
                        JoinDate = new DateTime(2019, 6, 1),
                        Status = DoctorStatus.Active,
                        ConsultationFee = 250.00m,
                        ConsultationDurationMinutes = 45,
                        AcceptsInsurance = true,
                        InsuranceProviders = "Blue Cross,Aetna",
                        Qualifications = "MBBS, MS (Orthopedics), FAAOS",
                        MedicalSchool = "Stanford University",
                        GraduationYear = 2008,
                        AddressLine1 = "789 Orthopedic Center",
                        City = "Chicago",
                        State = "IL",
                        Country = "USA",
                        PostalCode = "60601",
                        Languages = "English,Spanish",
                        Biography = "Specializes in sports injuries and joint replacement surgery.",
                        IsAvailableForTeleconsultation = false,
                        IsAcceptingNewPatients = true,
                        MaxPatientsPerDay = 15,
                        TotalPatientsSeen = 1800,
                        AverageRating = 4.7m,
                        TotalReviews = 120,
                        IsActive = true,
                        CreatedBy = Guid.Empty,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Doctors.AddRangeAsync(doctors);

                // Seed availabilities
                var availabilities = new List<DoctorAvailability>();
                foreach (var doctor in doctors)
                {
                    // Monday to Friday
                    for (int day = 1; day <= 5; day++)
                    {
                        availabilities.Add(new DoctorAvailability
                        {
                            Id = Guid.NewGuid(),
                            DoctorId = doctor.Id,
                            DayOfWeek = (DayOfWeek)day,
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(17, 0, 0),
                            SlotDurationMinutes = doctor.ConsultationDurationMinutes,
                            IsAvailable = true,
                            EffectiveFrom = DateTime.UtcNow.AddDays(-30),
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await context.DoctorAvailabilities.AddRangeAsync(availabilities);
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding database");
                throw;
            }
        }
    }
}

