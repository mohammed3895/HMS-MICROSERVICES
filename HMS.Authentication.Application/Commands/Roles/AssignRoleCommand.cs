using MediatR;

namespace HMS.Authentication.Application.Commands.Roles
{
    public class AssignRoleCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
