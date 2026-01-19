using HMS.Authentication.Application.DTOs.Roles;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Roles
{
    public class CreateRoleCommand : IRequest<Result<CreateRoleResponse>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystemRole { get; set; }
    }
}
