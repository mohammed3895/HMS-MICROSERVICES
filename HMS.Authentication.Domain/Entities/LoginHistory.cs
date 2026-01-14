namespace HMS.Authentication.Domain.Entities
{
    public class LoginHistory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsSuccessful { get; set; }
        public string? FailureReason { get; set; }
        public string? DeviceId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
