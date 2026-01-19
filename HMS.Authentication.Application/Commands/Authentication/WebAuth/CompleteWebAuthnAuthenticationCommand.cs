using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication.WebAuth
{
    public class CompleteWebAuthnAuthenticationCommand : IRequest<Result<WebAuthnAuthenticationResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string CredentialId { get; set; } = string.Empty;
        public string AuthenticatorData { get; set; } = string.Empty;
        public string ClientDataJSON { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
    }
}
