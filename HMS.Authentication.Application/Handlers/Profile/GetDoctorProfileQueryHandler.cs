using AutoMapper;
using HMS.Authentication.Application.DTOs.Profile;
using HMS.Authentication.Application.Queries.Profile;
using HMS.Authentication.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class GetDoctorProfileQueryHandler : IRequestHandler<GetDoctorProfileQuery, Result<DoctorProfileDto>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IMapper _mapper;

        public GetDoctorProfileQueryHandler(AuthenticationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<DoctorProfileDto>> Handle(
            GetDoctorProfileQuery request,
            CancellationToken cancellationToken)
        {
            var profile = await _context.DoctorProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return Result<DoctorProfileDto>.Failure("Doctor profile not found");

            var dto = _mapper.Map<DoctorProfileDto>(profile);
            return Result<DoctorProfileDto>.Success(dto);
        }
    }
}
