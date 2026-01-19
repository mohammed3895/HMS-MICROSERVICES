using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Profile
{
    public class GetPharmacistProfileQuery : IRequest<Result<PharmacistProfileDto>>
    {
        public Guid UserId { get; set; }
    }
}
