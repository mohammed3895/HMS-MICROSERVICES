using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication.WebAuth
{
    public class ListWebAuthnCredentialsCommand : IRequest<Result<List<WebAuthnCredentialInfo>>>
    {
        public Guid UserId { get; set; }
    }
}
