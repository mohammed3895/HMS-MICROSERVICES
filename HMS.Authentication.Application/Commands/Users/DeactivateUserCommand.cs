using MediatR;

namespace HMS.Authentication.Application.Commands.Users
{
    public class DeactivateUserCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
    }
}
