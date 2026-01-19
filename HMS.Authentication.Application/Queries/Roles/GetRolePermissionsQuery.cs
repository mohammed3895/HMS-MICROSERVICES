using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Roles
{
    public class GetRolePermissionsQuery : IRequest<Result<List<string>>>
    {
        public Guid RoleId { get; set; }
    }
}
