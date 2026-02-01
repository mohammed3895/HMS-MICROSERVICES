namespace HMS.Staff.Application.DTOs
{
    public class StaffLeaveDto
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
