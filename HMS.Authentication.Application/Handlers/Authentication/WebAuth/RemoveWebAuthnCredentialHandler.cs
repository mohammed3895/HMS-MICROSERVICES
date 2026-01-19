using HMS.Authentication.Application.Commands.Authentication.WebAuth;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication.WebAuth
{
    public class RemoveWebAuthnCredentialHandler
        : IRequestHandler<RemoveWebAuthnCredentialCommand, Result<Unit>>
    {
        private readonly IWebAuthnService _webAuthnService;
        private readonly ILogger<RemoveWebAuthnCredentialHandler> _logger;

        public RemoveWebAuthnCredentialHandler(
            IWebAuthnService webAuthnService,
            ILogger<RemoveWebAuthnCredentialHandler> logger)
        {
            _webAuthnService = webAuthnService;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(
            RemoveWebAuthnCredentialCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var success = await _webAuthnService.RevokeCredentialAsync(
                    request.UserId,
                    request.CredentialId,
                    "Removed by user");

                if (!success)
                {
                    return Result<Unit>.Failure("Credential not found or already revoked");
                }

                _logger.LogInformation(
                    "WebAuthn credential {CredentialId} removed for user {UserId}",
                    request.CredentialId, request.UserId);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error removing WebAuthn credential {CredentialId} for user {UserId}",
                    request.CredentialId, request.UserId);
                return Result<Unit>.Failure("An error occurred while removing the credential");
            }
        }
    }
}