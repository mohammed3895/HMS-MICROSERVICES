using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class Disable2FACommand : IRequest<Result<object>>
    {
        public Guid UserId { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
