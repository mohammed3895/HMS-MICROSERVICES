namespace HMS.Staff.Domain.Entities
{
    public class Specialization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }

        public ICollection<Staff> StaffMembers { get; set; }
    }

}
