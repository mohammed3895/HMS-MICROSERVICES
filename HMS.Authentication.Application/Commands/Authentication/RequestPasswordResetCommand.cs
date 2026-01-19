using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class RequestPasswordResetCommand : IRequest<Result<object>>
    {
        public string Email { get; set; }
    }
}
