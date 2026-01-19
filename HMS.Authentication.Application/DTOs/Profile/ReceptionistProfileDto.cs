namespace HMS.Authentication.Application.DTOs.Profile
{
    public class ReceptionistProfileDto
    {
        public Guid Id { get; set; }
        public string? Department { get; set; }
        public string? EmployeeId { get; set; }
        public decimal? Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Shift { get; set; }
        public string? Desk { get; set; }
    }
}
