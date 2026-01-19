using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdateProfilePictureCommand : IRequest<Result<string>>
    {
        public Guid UserId { get; set; }
        public IFormFile? File { get; set; }
    }
}
