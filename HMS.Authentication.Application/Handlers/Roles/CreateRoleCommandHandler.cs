using HMS.Authentication.Application.Commands.Roles;
using HMS.Authentication.Application.DTOs.Roles;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Domain.Enums;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMS.Authentication.Application.Handlers.Roles
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<CreateRoleResponse>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAuditService _auditService;

        public CreateRoleCommandHandler(
            RoleManager<ApplicationRole> roleManager,
            IAuditService auditService)
        {
            _roleManager = roleManager;
            _auditService = auditService;
        }

        public async Task<Result<CreateRoleResponse>> Handle(
            CreateRoleCommand request,
            CancellationToken cancellationToken)
        {
            var role = new ApplicationRole
            {
                Name = request.Name,
                Description = request.Description,
                IsSystemRole = request.IsSystemRole
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<CreateRoleResponse>.Failure(errors);
            }

            await _auditService.LogAsync(
                null,
                AuditAction.RoleCreated.ToString(),
                "Role",
                role.Id.ToString());

            var response = new CreateRoleResponse
            {
                RoleId = role.Id,
                Name = role.Name
            };

            return Result<CreateRoleResponse>.Success(response);
        }
    }
}
