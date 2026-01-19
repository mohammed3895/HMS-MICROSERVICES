namespace HMS.Authentication.Application.DTOs.Roles
{
    public class GetRoleResponse
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystemRole { get; set; }
        public List<string> Permissions { get; set; }
    }
}
