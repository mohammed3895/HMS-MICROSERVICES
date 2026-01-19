using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class LogoutCommand : IRequest<Result<object>>
    {
        public Guid UserId { get; set; }
        public bool LogoutAllDevices { get; set; }
    }
}
