using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Profile
{
    public class GetNurseProfileQuery : IRequest<Result<NurseProfileDto>>
    {
        public Guid UserId { get; set; }
    }
}
