using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Roles
{
    public class RevokePermissionCommand : IRequest<Result<Unit>>
    {
        public Guid RoleId { get; set; }
        public string Permission { get; set; }
    }
}
