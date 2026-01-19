using AutoMapper;
using HMS.Authentication.Application.Commands.Profile;
using HMS.Authentication.Application.DTOs.Profile;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class UpdatePharmacistProfileCommandHandler : IRequestHandler<UpdatePharmacistProfileCommand, Result<PharmacistProfileDto>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public UpdatePharmacistProfileCommandHandler(
            AuthenticationDbContext context,
            IMapper mapper,
            IAuditService auditService)
        {
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<Result<PharmacistProfileDto>> Handle(
            UpdatePharmacistProfileCommand request,
            CancellationToken cancellationToken)
        {
            var profile = await _context.PharmacistProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return Result<PharmacistProfileDto>.Failure("Pharmacist profile not found");

            if (request.Department != null) profile.Department = request.Department;
            if (request.PharmacyLocation != null) profile.PharmacyLocation = request.PharmacyLocation;

            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync("PharmacistProfileUpdated", "PharmacistProfile", profile.Id.ToString(),
                "Pharmacist profile information updated", request.UserId.ToString());

            var dto = _mapper.Map<PharmacistProfileDto>(profile);
            return Result<PharmacistProfileDto>.Success(dto);
        }
    }
}
