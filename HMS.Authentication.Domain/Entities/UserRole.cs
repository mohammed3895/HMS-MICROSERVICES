namespace HMS.Authentication.Domain.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public DateTime AssignedAt { get; set; }
        public Guid AssignedBy { get; set; }

        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
