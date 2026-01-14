using MediatR;

namespace HMS.Authentication.Application.Commands.Roles
{
    public class DeleteRoleCommand : IRequest<Result<Unit>>
    {
        public Guid RoleId { get; set; }
    }
}
