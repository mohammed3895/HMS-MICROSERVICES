using HMS.Authentication.Application.DTOs.Roles;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Roles
{
    public class GetRoleByIdQuery : IRequest<Result<GetRoleResponse>>
    {
        public Guid RoleId { get; set; }
    }
}
