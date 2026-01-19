using Fido2NetLib;
using Fido2NetLib.Objects;
using HMS.Authentication.Application.Commands.Authentication.WebAuth;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication.WebAuth
{
    public class CompleteWebAuthnRegistrationHandler
        : IRequestHandler<CompleteWebAuthnRegistrationCommand, Result<Unit>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebAuthnService _webAuthnService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CompleteWebAuthnRegistrationHandler> _logger;

        public CompleteWebAuthnRegistrationHandler(
            AuthenticationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebAuthnService webAuthnService,
            IAuditService auditService,
            ILogger<CompleteWebAuthnRegistrationHandler> logger)
        {
            _context = context;
            _userManager = userManager;
            _webAuthnService = webAuthnService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(
            CompleteWebAuthnRegistrationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return Result<Unit>.Failure("User not found");
                }

                // Build AuthenticatorAttestationRawResponse for Fido2
                var attestationResponse = new AuthenticatorAttestationRawResponse
                {
                    Id = request.CredentialId,
                    RawId = Convert.FromBase64String(request.CredentialId),
                    Response = new AuthenticatorAttestationRawResponse.AttestationResponse
                    {
                        ClientDataJson = Convert.FromBase64String(request.ClientDataJSON),
                        AttestationObject = Convert.FromBase64String(request.AttestationObject)
                    },
                    Type = PublicKeyCredentialType.PublicKey
                };

                // Complete registration using WebAuthnService
                var credential = await _webAuthnService.CompleteRegistrationAsync(
                    user,
                    attestationResponse,
                    request.DeviceName);

                if (credential == null)
                {
                    _logger.LogWarning("WebAuthn registration verification failed for user {UserId}",
                        request.UserId);
                    return Result<Unit>.Failure("Failed to register credential");
                }

                // Enable WebAuthn for user if this is their first credential
                if (!user.IsWebAuthnEnabled)
                {
                    user.IsWebAuthnEnabled = true;
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Log the activity
                await _auditService.LogAsync(
                    "WebAuthnCredentialRegistered",
                    "Authentication",
                    request.UserId.ToString(),
                    credential.DeviceName ?? "Unknown Device",
                    $"Type: {credential.CredType}, AAGUID: {credential.AaGuid}");

                _logger.LogInformation(
                    "WebAuthn credential registered successfully for user {UserId}: {DeviceName}",
                    request.UserId, credential.DeviceName);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing WebAuthn registration for user {UserId}",
                    request.UserId);
                return Result<Unit>.Failure("An error occurred during registration");
            }
        }
    }
}