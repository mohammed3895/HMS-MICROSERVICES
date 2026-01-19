using HMS.Authentication.Application.Commands.Profile;
using HMS.Authentication.Application.DTOs.Profile;
using HMS.Authentication.Application.Queries.Profile;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class UpdateContactInfoCommandHandler : IRequestHandler<UpdateContactInfoCommand, Result<UserProfileResponse>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IMediator _mediator;

        public UpdateContactInfoCommandHandler(
            AuthenticationDbContext context,
            IAuditService auditService,
            IMediator mediator)
        {
            _context = context;
            _auditService = auditService;
            _mediator = mediator;
        }

        public async Task<Result<UserProfileResponse>> Handle(
            UpdateContactInfoCommand request,
            CancellationToken cancellationToken)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (profile == null)
            {
                profile = new UserProfile { UserId = request.UserId };
                _context.UserProfiles.Add(profile);
            }

            // Update fields
            if (request.Address != null) profile.Address = request.Address;
            if (request.City != null) profile.City = request.City;
            if (request.State != null) profile.State = request.State;
            if (request.ZipCode != null) profile.ZipCode = request.ZipCode;
            if (request.Country != null) profile.Country = request.Country;
            if (request.EmergencyContactName != null) profile.EmergencyContactName = request.EmergencyContactName;
            if (request.EmergencyContactPhone != null) profile.EmergencyContactPhone = request.EmergencyContactPhone;
            if (request.BloodGroup != null) profile.BloodGroup = request.BloodGroup;
            if (request.Allergies != null) profile.Allergies = request.Allergies;

            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync("ContactInfoUpdated", "UserProfile", profile.Id.ToString(),
                "Contact information updated", request.UserId.ToString());

            var profileQuery = new GetUserProfileQuery { UserId = request.UserId };
            return await _mediator.Send(profileQuery, cancellationToken);
        }
    }
}
