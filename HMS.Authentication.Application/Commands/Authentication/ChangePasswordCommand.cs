using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class ChangePasswordCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
