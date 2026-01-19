using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace HMS.Authentication.Application.Handlers.Authentication._2FA
{
    public class UseRecoveryCodeCommandHandler : IRequestHandler<UseRecoveryCodeCommand, Result<LoginResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAuditService _auditService;
        private readonly IEmailService _emailService;
        private readonly ILogger<UseRecoveryCodeCommandHandler> _logger;

        public UseRecoveryCodeCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            ITokenService tokenService,
            IAuditService auditService,
            IEmailService emailService,
            ILogger<UseRecoveryCodeCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _auditService = auditService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Handle(UseRecoveryCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<LoginResponse>.Failure("Invalid recovery code");
                }

                if (!user.IsTwoFactorEnabled)
                {
                    _logger.LogWarning("2FA not enabled for user: {UserId}", request.UserId);
                    return Result<LoginResponse>.Failure("Two-factor authentication is not enabled");
                }

                // Validate recovery code
                if (user.TwoFactorRecoveryCodes == null || !user.TwoFactorRecoveryCodes.Contains(request.RecoveryCode))
                {
                    _logger.LogWarning("Invalid recovery code for user: {UserId}", request.UserId);
                    await _auditService.LogAsync("2FA_RecoveryCodeFailed", "User", user.Id.ToString(),
                        "Invalid recovery code attempt", null);
                    return Result<LoginResponse>.Failure("Invalid recovery code");
                }

                // Remove used recovery code
                user.TwoFactorRecoveryCodes.Remove(request.RecoveryCode);
                await _userManager.UpdateAsync(user);

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
                await _auditService.LogAsync("2FA_RecoveryCodeUsed", "User", user.Id.ToString(),
                    "Recovery code used successfully", null);

                // Send notification email
                await _emailService.SendRecoveryCodeUsedEmailAsync(user);

                _logger.LogInformation("User logged in with recovery code: {UserId}", user.Id);

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
                    Message = "Login successful using recovery code",
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
                _logger.LogError(ex, "Error using recovery code for user: {UserId}", request.UserId);
                return Result<LoginResponse>.Failure("An error occurred while processing the recovery code");
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