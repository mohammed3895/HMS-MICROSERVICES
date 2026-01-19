using AutoMapper;
using HMS.Authentication.Application.Commands.Users;
using HMS.Authentication.Application.DTOs.Users;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Domain.Enums;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMS.Authentication.Application.Handlers.Roles
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public CreateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IAuditService auditService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<Result<CreateUserResponse>> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                NationalId = request.NationalId,
                LicenseNumber = request.LicenseNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                await _auditService.LogAsync(
                    AuditAction.UserCreated.ToString(),
                    "User",
                    user.Id.ToString(),
                    null,
                    errors);
                return Result<CreateUserResponse>.Failure(errors);
            }

            var profile = new UserProfile { UserId = user.Id };
            // Save profile (to be implemented with DbContext)

            await _auditService.LogAsync(
                user.Id.ToString(),
                AuditAction.UserCreated.ToString(),
                "User",
                user.Id.ToString());

            var response = new CreateUserResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}"
            };

            return Result<CreateUserResponse>.Success(response);
        }
    }
}
