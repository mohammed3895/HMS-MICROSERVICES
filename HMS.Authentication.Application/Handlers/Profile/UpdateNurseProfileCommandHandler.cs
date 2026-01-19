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
    public class UpdateNurseProfileCommandHandler : IRequestHandler<UpdateNurseProfileCommand, Result<NurseProfileDto>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public UpdateNurseProfileCommandHandler(
            AuthenticationDbContext context,
            IMapper mapper,
            IAuditService auditService)
        {
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<Result<NurseProfileDto>> Handle(
            UpdateNurseProfileCommand request,
            CancellationToken cancellationToken)
        {
            var profile = await _context.NurseProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return Result<NurseProfileDto>.Failure("Nurse profile not found");

            if (request.Department != null) profile.Department = request.Department;
            if (request.Specialization != null) profile.Specialization = request.Specialization;
            if (request.Shift != null) profile.Shift = request.Shift;
            if (request.Ward != null) profile.Ward = request.Ward;

            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync("NurseProfileUpdated", "NurseProfile", profile.Id.ToString(),
                "Nurse profile information updated", request.UserId.ToString());

            var dto = _mapper.Map<NurseProfileDto>(profile);
            return Result<NurseProfileDto>.Success(dto);
        }
    }
}
