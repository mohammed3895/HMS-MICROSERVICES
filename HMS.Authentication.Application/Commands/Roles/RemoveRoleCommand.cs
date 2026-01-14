using MediatR;

namespace HMS.Authentication.Application.Commands.Roles
{
    public class RemoveRoleCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
