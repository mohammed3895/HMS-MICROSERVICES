using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class LogoutCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
    }
}
