namespace HMS.Staff.Domain.Entities
{
    public class Bed
    {
        public Guid Id { get; set; }
        public string BedNumber { get; set; } = string.Empty;
        public Guid WardId { get; set; }
        public string? BedType { get; set; } // Standard, ICU, Isolation
        public bool IsOccupied { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Ward? Ward { get; set; }
        public virtual ICollection<PatientAdmission> PatientAdmissions { get; set; } = [];
    }
}
