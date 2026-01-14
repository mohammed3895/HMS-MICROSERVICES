namespace HMS.Authentication.Domain.Entities
{
    public class RolePermission
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Permission { get; set; } // e.g., "Patient.Read", "Patient.Write"
        public DateTime GrantedAt { get; set; }

        public ApplicationRole Role { get; set; }
    }
}
