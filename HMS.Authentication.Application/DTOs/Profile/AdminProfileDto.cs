namespace HMS.Authentication.Application.DTOs.Profile
{
    public class AdminProfileDto
    {
        public Guid Id { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTime JoiningDate { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
