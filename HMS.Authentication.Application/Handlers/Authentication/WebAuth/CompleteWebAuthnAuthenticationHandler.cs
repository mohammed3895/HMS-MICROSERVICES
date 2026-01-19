using Fido2NetLib;
using Fido2NetLib.Objects;
using HMS.Authentication.Application.Commands.Authentication.WebAuth;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication.WebAuth
{
    public class CompleteWebAuthnAuthenticationHandler
        : IRequestHandler<CompleteWebAuthnAuthenticationCommand, Result<WebAuthnAuthenticationResponse>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebAuthnService _webAuthnService;
        private readonly ITokenService _tokenService;
        private readonly IDeviceService _deviceService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CompleteWebAuthnAuthenticationHandler> _logger;

        public CompleteWebAuthnAuthenticationHandler(
            AuthenticationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebAuthnService webAuthnService,
            ITokenService tokenService,
            IDeviceService deviceService,
            IAuditService auditService,
            ILogger<CompleteWebAuthnAuthenticationHandler> logger)
        {
            _context = context;
            _userManager = userManager;
            _webAuthnService = webAuthnService;
            _tokenService = tokenService;
            _deviceService = deviceService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<WebAuthnAuthenticationResponse>> Handle(
            CompleteWebAuthnAuthenticationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Find user by email
                var user = await _userManager.Users
                    .Include(u => u.WebAuthnCredentials)
                    .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("WebAuthn authentication completion for non-existent email: {Email}",
                        request.Email);
                    return Result<WebAuthnAuthenticationResponse>.Failure("Invalid credentials");
                }

                // Get active credentials
                var credentials = user.WebAuthnCredentials
                    .Where(c => !c.IsRevoked)
                    .ToList();

                // Build AuthenticatorAssertionRawResponse for Fido2
                var assertionResponse = new AuthenticatorAssertionRawResponse
                {
                    Id = request.CredentialId,
                    RawId = Convert.FromBase64String(request.CredentialId),
                    Response = new AuthenticatorAssertionRawResponse.AssertionResponse
                    {
                        AuthenticatorData = Convert.FromBase64String(request.AuthenticatorData),
                        ClientDataJson = Convert.FromBase64String(request.ClientDataJSON),
                        Signature = Convert.FromBase64String(request.Signature)
                    },
                    Type = PublicKeyCredentialType.PublicKey
                };

                // Verify the assertion using WebAuthnService
                var (success, credential) = await _webAuthnService.CompleteAuthenticationAsync(
                    user,
                    assertionResponse,
                    credentials);

                if (!success || credential == null)
                {
                    _logger.LogWarning("WebAuthn authentication failed for user {UserId}", user.Id);

                    await _auditService.LogAsync(
                        "WebAuthnAuthenticationFailed",
                        "Security",
                        user.Id.ToString(),
                        "Failed authentication attempt",
                        null);

                    return Result<WebAuthnAuthenticationResponse>.Failure(
                        "Authentication failed. Please try again.");
                }

                // Register or update device
                if (!string.IsNullOrEmpty(request.DeviceId))
                {
                    var device = await _deviceService.RegisterOrUpdateDeviceAsync(
                        user.Id,
                        request.DeviceId);

                    // Trust the device separately
                    await _deviceService.TrustDeviceAsync(user.Id, request.DeviceId);
                }

                // Get user roles for token generation
                var roles = await _userManager.GetRolesAsync(user);

                // Generate access token with await
                var accessToken = await _tokenService.GenerateAccessTokenAsync(
                    user,
                    roles.ToList());

                // Generate refresh token (synchronous method)
                var refreshToken = _tokenService.GenerateRefreshToken();

                var expiresAt = DateTime.UtcNow.AddHours(1);

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                // Log successful authentication
                await _auditService.LogAsync(
                    "WebAuthnLoginSuccess",
                    "Authentication",
                    user.Id.ToString(),
                    credential.DeviceName ?? "Unknown Device",
                    null);

                _logger.LogInformation(
                    "WebAuthn authentication successful for user {UserId} using {DeviceName}",
                    user.Id, credential.DeviceName);

                return Result<WebAuthnAuthenticationResponse>.Success(new WebAuthnAuthenticationResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing WebAuthn authentication for {Email}",
                    request.Email);
                return Result<WebAuthnAuthenticationResponse>.Failure(
                    "An error occurred during authentication");
            }
        }
    }
}