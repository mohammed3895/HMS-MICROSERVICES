using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class Enable2FACommand : IRequest<Result<Enable2FAResponse>>
    {
        public Guid UserId { get; set; }
    }
}
