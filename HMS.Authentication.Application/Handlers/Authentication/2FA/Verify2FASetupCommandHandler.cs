using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OtpNet;
using System.Security.Cryptography;

namespace HMS.Authentication.Application.Handlers.Authentication._2FA
{
    public class Verify2FASetupCommandHandler : IRequestHandler<Verify2FASetupCommand, Result<Verify2FASetupResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;
        private readonly IEmailService _emailService;
        private readonly ILogger<Verify2FASetupCommandHandler> _logger;

        public Verify2FASetupCommandHandler(
            UserManager<ApplicationUser> userManager,
            IAuditService auditService,
            IEmailService emailService,
            ILogger<Verify2FASetupCommandHandler> logger)
        {
            _userManager = userManager;
            _auditService = auditService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result<Verify2FASetupResponse>> Handle(Verify2FASetupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<Verify2FASetupResponse>.Failure("User not found");
                }

                if (string.IsNullOrEmpty(user.TwoFactorSecret))
                {
                    _logger.LogWarning("2FA secret not initialized for user: {UserId}", request.UserId);
                    return Result<Verify2FASetupResponse>.Failure("Two-factor authentication setup not initialized");
                }

                // Verify TOTP code
                var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret));
                var isValid = totp.VerifyTotp(request.Code, out _, new VerificationWindow(2, 2));

                if (!isValid)
                {
                    _logger.LogWarning("Invalid 2FA setup code for user: {UserId}", request.UserId);
                    await _auditService.LogAsync("2FA_SetupFailed", "User", user.Id.ToString(),
                        "Invalid verification code during 2FA setup", null);
                    return Result<Verify2FASetupResponse>.Failure("Invalid verification code");
                }

                // Enable 2FA for user
                user.IsTwoFactorEnabled = true;

                // Generate recovery codes
                var recoveryCodes = GenerateRecoveryCodes(10);
                user.TwoFactorRecoveryCodes = recoveryCodes;

                await _userManager.UpdateAsync(user);

                // Log and notify
                await _auditService.LogAsync("2FA_Enabled", "User", user.Id.ToString(),
                    "Two-factor authentication enabled successfully", null);
                await _emailService.Send2FAEnabledEmailAsync(user);

                _logger.LogInformation("2FA enabled for user: {UserId}", user.Id);

                var response = new Verify2FASetupResponse
                {
                    IsVerified = true,
                    RecoveryCodes = recoveryCodes,
                    Message = "Two-factor authentication has been successfully enabled"
                };

                return Result<Verify2FASetupResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying 2FA setup for user: {UserId}", request.UserId);
                return Result<Verify2FASetupResponse>.Failure("An error occurred during 2FA setup verification");
            }
        }

        private List<string> GenerateRecoveryCodes(int count)
        {
            var codes = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var bytes = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }
                var code = Convert.ToBase64String(bytes)
                    .Replace("+", "")
                    .Replace("/", "")
                    .Replace("=", "")
                    .Substring(0, 10)
                    .ToUpper();
                codes.Add(code);
            }
            return codes;
        }
    }
}