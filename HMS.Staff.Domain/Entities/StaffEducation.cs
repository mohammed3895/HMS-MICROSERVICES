namespace HMS.Staff.Domain.Entities
{
    public class StaffEducation
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }

        public Staff Staff { get; set; } = null!;
    }
}
