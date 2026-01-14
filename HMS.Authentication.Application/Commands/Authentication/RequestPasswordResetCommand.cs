using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class RequestPasswordResetCommand : IRequest<Result<Unit>>
    {
        public string Email { get; set; }
    }
}
