using HMS.Authentication.Domain.Enums;

namespace HMS.Authentication.Domain.Entities
{
    public class AuditStatistics
    {
        public string Id { get; set; } = null!;
        public int TotalEvents { get; set; }
        public int SuccessfulEvents { get; set; }
        public int FailedEvents { get; set; }
        public Dictionary<AuditAction, int> EventsByAction { get; set; } = new();
        public Dictionary<string, int> EventsByEntity { get; set; } = new();
        public Dictionary<Guid, int> EventsByUser { get; set; } = new();
        public int UniqueUsers { get; set; }
        public DateTime? EarliestEvent { get; set; }
        public DateTime? LatestEvent { get; set; }
    }
}
