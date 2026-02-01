namespace HMS.Staff.Domain.Entities
{
    public class StaffPerformanceReview
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public Guid ReviewerId { get; set; }
        public DateTime ReviewDate { get; set; }
        public string ReviewPeriod { get; set; } = string.Empty;
        public int OverallRating { get; set; } // 1-5
        public string? Strengths { get; set; }
        public string? AreasForImprovement { get; set; }
        public string? Goals { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; }

        public Staff Staff { get; set; } = null!;
    }
}
