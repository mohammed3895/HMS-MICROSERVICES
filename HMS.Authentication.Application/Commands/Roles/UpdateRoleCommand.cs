using MediatR;

namespace HMS.Authentication.Application.Commands.Roles
{
    public class UpdateRoleCommand : IRequest<Result<Unit>>
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
