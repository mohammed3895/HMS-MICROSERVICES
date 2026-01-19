using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Users
{
    public class ActivateUserCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
    }
}
