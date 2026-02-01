using HMS.Staff.Domain.Enums;

namespace HMS.Staff.Domain.Entities
{
    public class StaffLeave
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }

        public Staff Staff { get; set; } = null!;
    }
}
