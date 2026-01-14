using HMS.Authentication.Domain.Entities;
using HMS.Staff.Domain.Enums;

namespace HMS.Staff.Domain.Entities
{
    public class Staff
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string EmployeeId { get; set; }
        public StaffType StaffType { get; set; } // Doctor, Nurse, Technician, etc.
        public Guid DepartmentId { get; set; }
        public Guid? SpecializationId { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Qualifications { get; set; }
        public StaffStatus Status { get; set; }
        public string? ShiftPattern { get; set; }
        public decimal? HourlyRate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Department Department { get; set; }
        public Specialization? Specialization { get; set; }
        public ICollection<StaffSchedule> Schedules { get; set; }
        public ICollection<StaffLeave> Leaves { get; set; }
        public ApplicationUser User { get; set; }
    }
}
