namespace HMS.Common.Entites
{
    public class BaseEntity
    {
        public string Id { get; set; } = new Guid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
