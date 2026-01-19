using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdatePharmacistProfileCommand : IRequest<Result<PharmacistProfileDto>>
    {
        public Guid UserId { get; set; }
        public string? Department { get; set; }
        public string? PharmacyLocation { get; set; }
    }
}
