using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication.WebAuth
{
    public class CompleteWebAuthnRegistrationCommand : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public string CredentialId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string ClientDataJSON { get; set; } = string.Empty;
        public string AttestationObject { get; set; } = string.Empty;
        public string? DeviceName { get; set; }
    }
}
