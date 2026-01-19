using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication.WebAuth
{
    public class RemoveWebAuthnCredentialCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public Guid CredentialId { get; set; }
    }
}
