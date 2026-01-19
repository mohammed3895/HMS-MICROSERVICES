using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Profile
{
    public class GetDoctorProfileQuery : IRequest<Result<DoctorProfileDto>>
    {
        public Guid UserId { get; set; }
    }
}
