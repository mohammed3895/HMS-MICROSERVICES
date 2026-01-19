using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication.WebAuth
{
    public class BeginWebAuthnAuthenticationCommand : IRequest<Result<WebAuthnAuthenticationOptions>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
