namespace HMS.Staff.Application.DTOs
{
    public class CreateStaffAuthResponse
    {
        public bool Success { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
