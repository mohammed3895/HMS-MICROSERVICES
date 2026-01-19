using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OtpNet;
using System.Security.Cryptography;

namespace HMS.Authentication.Application.Handlers.Authentication._2FA
{
    public class Verify2FACodeCommandHandler : IRequestHandler<Verify2FACodeCommand, Result<LoginResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAuditService _auditService;
        private readonly ILogger<Verify2FACodeCommandHandler> _logger;

        public Verify2FACodeCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            ITokenService tokenService,
            IAuditService auditService,
            ILogger<Verify2FACodeCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Handle(Verify2FACodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<LoginResponse>.Failure("Invalid 2FA code");
                }

                if (!user.IsTwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
                {
                    _logger.LogWarning("2FA not properly configured for user: {UserId}", request.UserId);
                    return Result<LoginResponse>.Failure("Two-factor authentication is not enabled");
                }

                // Validate 2FA token
                var isTokenValid = await _tokenService.Validate2FATokenAsync(user.Id, request.TwoFactorToken);
                if (!isTokenValid)
                {
                    _logger.LogWarning("Invalid 2FA token for user: {UserId}", request.UserId);
                    return Result<LoginResponse>.Failure("Invalid or expired 2FA session");
                }

                // Verify TOTP code
                var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret));
                var isValid = totp.VerifyTotp(request.Code, out _, new VerificationWindow(2, 2));

                if (!isValid)
                {
                    _logger.LogWarning("Invalid 2FA code for user: {UserId}", request.UserId);
                    await _auditService.LogAsync("2FA_CodeFailed", "User", user.Id.ToString(),
                        "Invalid 2FA code attempt", null);
                    return Result<LoginResponse>.Failure("Invalid verification code");
                }

                // Generate tokens
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = await _tokenService.GenerateAccessTokenAsync(user, roles.ToList());
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

                // Update user refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                // ✅ FIX: Generate unique SessionToken
                var sessionToken = GenerateSecureToken(32);

                // Create user session
                var session = new UserSession
                {
                    UserId = user.Id,
                    DeviceId = request.DeviceId,
                    SessionToken = sessionToken, // ✅ Added this field
                    RefreshToken = refreshToken,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastActivityAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync(cancellationToken);

                // Log successful login
                await _auditService.LogAsync("2FA_Success", "User", user.Id.ToString(),
                    $"2FA verification successful. Device trusted: {request.TrustDevice}", null);

                _logger.LogInformation("User logged in with 2FA: {UserId}", user.Id);

                // Return login response
                var response = new LoginResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    RequiresTwoFactor = false,
                    RequiresEmailVerification = false,
                    RequiresOtp = false,
                    Message = "Login successful",
                    UserInfo = new UserInfoDto
                    {
                        UserId = user.Id,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = roles.ToList()
                    }
                };

                return Result<LoginResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying 2FA code for user: {UserId}", request.UserId);
                return Result<LoginResponse>.Failure("An error occurred during 2FA verification");
            }
        }

        /// <summary>
        /// Generate a cryptographically secure random token
        /// </summary>
        private string GenerateSecureToken(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}