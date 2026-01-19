using AutoMapper;
using HMS.Authentication.Application.Commands.Profile;
using HMS.Authentication.Application.DTOs.Profile;
using HMS.Authentication.Application.Queries.Profile;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class UpdateBasicProfileCommandHandler : IRequestHandler<UpdateBasicProfileCommand, Result<UserProfileResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateBasicProfileCommandHandler> _logger;

        public UpdateBasicProfileCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            IMapper mapper,
            IAuditService auditService,
            ILogger<UpdateBasicProfileCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<UserProfileResponse>> Handle(
            UpdateBasicProfileCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return Result<UserProfileResponse>.Failure("User not found");

            // Update fields
            if (!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName;

            if (!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName;

            if (request.DateOfBirth.HasValue)
                user.DateOfBirth = request.DateOfBirth.Value;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.NationalId))
                user.NationalId = request.NationalId;

            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<UserProfileResponse>.Failure($"Update failed: {errors}");
            }

            await _auditService.LogAsync("ProfileUpdated", "User", user.Id.ToString(),
                "Basic profile information updated", user.Id.ToString());

            // Get complete profile
            var mediator = _context.GetService<IMediator>();
            var profileQuery = new GetUserProfileQuery { UserId = user.Id };
            return await mediator!.Send(profileQuery, cancellationToken);
        }
    }
}
