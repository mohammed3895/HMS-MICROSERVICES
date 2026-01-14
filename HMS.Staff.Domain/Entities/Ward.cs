namespace HMS.Staff.Domain.Entities
{
    public class Ward
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid DepartmentId { get; set; }
        public int Capacity { get; set; }
        public int OccupiedBeds { get; set; }
        public string? WardType { get; set; } // ICU, General, Private, Semi-Private
        public string? PhoneExtension { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual Department? Department { get; set; }
        public virtual ICollection<Bed> Beds { get; set; } = [];
        public virtual ICollection<PatientAdmission> PatientAdmissions { get; set; } = [];
    }
}
