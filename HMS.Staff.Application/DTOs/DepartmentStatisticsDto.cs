namespace HMS.Staff.Application.DTOs
{
    public class DepartmentStatisticsDto
    {
        public string Department { get; set; } = string.Empty;
        public int TotalStaff { get; set; }
        public int ActiveStaff { get; set; }
        public int OnLeaveStaff { get; set; }
        public Dictionary<string, int> StaffByType { get; set; } = new();
    }
}
