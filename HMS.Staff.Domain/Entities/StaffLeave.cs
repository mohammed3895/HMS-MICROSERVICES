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
        public string Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? Comments { get; set; }

        public Staff Staff { get; set; }
    }
}
