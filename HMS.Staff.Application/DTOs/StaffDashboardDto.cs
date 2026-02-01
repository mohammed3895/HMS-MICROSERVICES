namespace HMS.Staff.Application.DTOs
{
    public class StaffDashboardDto
    {
        public StaffSummaryDto StaffInfo { get; set; } = null!;
        public int TotalWorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int PendingLeaveRequests { get; set; }
        public decimal AttendancePercentage { get; set; }
    }
}
