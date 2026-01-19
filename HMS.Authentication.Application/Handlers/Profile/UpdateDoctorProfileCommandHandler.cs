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
    public class UpdateDoctorProfileCommandHandler : IRequestHandler<UpdateDoctorProfileCommand, Result<DoctorProfileDto>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public UpdateDoctorProfileCommandHandler(
            AuthenticationDbContext context,
            IMapper mapper,
            IAuditService auditService)
        {
            _context = context;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<Result<DoctorProfileDto>> Handle(
            UpdateDoctorProfileCommand request,
            CancellationToken cancellationToken)
        {
            var profile = await _context.DoctorProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return Result<DoctorProfileDto>.Failure("Doctor profile not found");

            // Update fields
            if (request.Department != null) profile.Department = request.Department;
            if (request.Specialization != null) profile.Specialization = request.Specialization;
            if (request.Qualification != null) profile.Qualification = request.Qualification;
            if (request.Certifications != null) profile.Certifications = request.Certifications;
            if (request.ConsultationFee != null) profile.ConsultationFee = request.ConsultationFee;
            if (request.MaxPatientsPerDay.HasValue) profile.MaxPatientsPerDay = (int)request.MaxPatientsPerDay;
            if (request.Biography != null) profile.Biography = request.Biography;
            if (request.IsAvailableForConsultation.HasValue)
                profile.IsAvailable = request.IsAvailableForConsultation.Value;

            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync("DoctorProfileUpdated", "DoctorProfile", profile.Id.ToString(),
                "Doctor profile information updated", request.UserId.ToString());

            var dto = _mapper.Map<DoctorProfileDto>(profile);
            return Result<DoctorProfileDto>.Success(dto);
        }
    }
}
