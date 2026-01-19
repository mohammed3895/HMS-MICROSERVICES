using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdateNurseProfileCommand : IRequest<Result<NurseProfileDto>>
    {
        public Guid UserId { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? Shift { get; set; }
        public string? Ward { get; set; }
    }
}
