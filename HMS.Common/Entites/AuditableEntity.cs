namespace HMS.Common.Entites
{
    public class AuditableEntity : BaseEntity
    {
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
