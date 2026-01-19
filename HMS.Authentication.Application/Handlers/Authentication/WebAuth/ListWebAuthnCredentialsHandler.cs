using HMS.Authentication.Application.Commands.Authentication.WebAuth;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication.WebAuth
{
    public class ListWebAuthnCredentialsHandler
        : IRequestHandler<ListWebAuthnCredentialsCommand, Result<List<WebAuthnCredentialInfo>>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly ILogger<ListWebAuthnCredentialsHandler> _logger;

        public ListWebAuthnCredentialsHandler(
            AuthenticationDbContext context,
            ILogger<ListWebAuthnCredentialsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<WebAuthnCredentialInfo>>> Handle(
            ListWebAuthnCredentialsCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var credentials = await _context.UserCredentials
                    .Where(c => c.UserId == request.UserId && !c.IsRevoked)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new WebAuthnCredentialInfo
                    {
                        Id = c.Id,
                        DeviceName = c.DeviceName,
                        CreatedAt = c.CreatedAt,
                        SignatureCounter = c.SignatureCounter,
                        CredType = c.CredType,
                        IsBackupEligible = c.IsBackupEligible,
                        IsBackedUp = c.IsBackedUp,
                        LastUsedAt = c.LastUsedAt
                    })
                    .ToListAsync(cancellationToken);

                return Result<List<WebAuthnCredentialInfo>>.Success(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing WebAuthn credentials for user {UserId}",
                    request.UserId);
                return Result<List<WebAuthnCredentialInfo>>.Failure(
                    "An error occurred while retrieving credentials");
            }
        }
    }
}