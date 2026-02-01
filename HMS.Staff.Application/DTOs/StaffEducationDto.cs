namespace HMS.Staff.Application.DTOs
{
    public class StaffEducationDto
    {
        public Guid Id { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Country { get; set; }
    }
}
