using HMS.Authentication.Application.DTOs.Roles;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Roles
{
    public class GetAllRolesQuery : IRequest<Result<List<GetRoleResponse>>>
    {
        public bool? IncludeSystemRoles { get; set; }
    }
}
