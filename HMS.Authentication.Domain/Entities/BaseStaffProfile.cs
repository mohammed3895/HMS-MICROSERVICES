namespace HMS.Authentication.Domain.Entities
{
    public abstract class BaseStaffProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? EmployeeId { get; set; }
        public decimal? Salary { get; set; }
        public string? Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? OfficeLocation { get; set; }
        public string? WorkSchedule { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
