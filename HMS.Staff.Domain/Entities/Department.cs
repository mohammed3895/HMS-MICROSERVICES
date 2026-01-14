namespace HMS.Staff.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public Guid? HeadOfDepartmentId { get; set; }
        public string? PhoneExtension { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Staff> StaffMembers { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }
}
