namespace HMS.Staff.Domain.Entities
{
    public class StaffExperience
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string Organization { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? Department { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentPosition { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public Staff Staff { get; set; } = null!;
    }
}
