using AutoMapper;
using HMS.Authentication.Application.DTOs.Profile;
using HMS.Authentication.Application.Queries.Profile;
using HMS.Authentication.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class GetPharmacistProfileQueryHandler : IRequestHandler<GetPharmacistProfileQuery, Result<PharmacistProfileDto>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IMapper _mapper;

        public GetPharmacistProfileQueryHandler(AuthenticationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PharmacistProfileDto>> Handle(
            GetPharmacistProfileQuery request,
            CancellationToken cancellationToken)
        {
            var profile = await _context.PharmacistProfiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return Result<PharmacistProfileDto>.Failure("Pharmacist profile not found");

            var dto = _mapper.Map<PharmacistProfileDto>(profile);
            return Result<PharmacistProfileDto>.Success(dto);
        }
    }
}
