using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OtpNet;

namespace HMS.Authentication.Application.Handlers.Authentication._2FA
{
    public class Enable2FACommandHandler : IRequestHandler<Enable2FACommand, Result<Enable2FAResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;
        private readonly ILogger<Enable2FACommandHandler> _logger;

        public Enable2FACommandHandler(
            UserManager<ApplicationUser> userManager,
            IAuditService auditService,
            ILogger<Enable2FACommandHandler> logger)
        {
            _userManager = userManager;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<Enable2FAResponse>> Handle(Enable2FACommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<Enable2FAResponse>.Failure("User not found");
                }

                if (user.IsTwoFactorEnabled)
                {
                    _logger.LogWarning("2FA already enabled for user: {UserId}", request.UserId);
                    return Result<Enable2FAResponse>.Failure("Two-factor authentication is already enabled");
                }

                // Generate secret key (20 bytes = 160 bits)
                var key = KeyGeneration.GenerateRandomKey(20);
                var secret = Base32Encoding.ToString(key);

                // Format manual entry key (groups of 4 characters for easier reading)
                var manualEntryKey = FormatManualEntryKey(secret);

                // Generate QR code URL
                var qrCodeUrl = GenerateQrCodeUrl(user.Email!, secret);

                // Store secret temporarily (2FA not enabled until verification)
                user.TwoFactorSecret = secret;
                // Don't generate recovery codes yet - wait until verification
                user.TwoFactorRecoveryCodes = new List<string>();

                await _userManager.UpdateAsync(user);

                await _auditService.LogAsync(
                    "2FA_Initiated",
                    "User",
                    user.Id.ToString(),
                    "2FA setup initiated",
                    null);

                _logger.LogInformation("2FA setup initiated for user: {UserId}", user.Id);

                return Result<Enable2FAResponse>.Success(new Enable2FAResponse
                {
                    Secret = secret,
                    QrCodeUrl = qrCodeUrl,
                    ManualEntryKey = manualEntryKey
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling 2FA for user: {UserId}", request.UserId);
                return Result<Enable2FAResponse>.Failure("An error occurred while setting up two-factor authentication");
            }
        }

        private string GenerateQrCodeUrl(string email, string secret)
        {
            // URI format: otpauth://totp/[Issuer]:[AccountName]?secret=[Secret]&issuer=[Issuer]
            var issuer = "Hospital Management System";
            var accountName = System.Web.HttpUtility.UrlEncode(email);
            var issuerEncoded = System.Web.HttpUtility.UrlEncode(issuer);

            return $"otpauth://totp/{issuerEncoded}:{accountName}?secret={secret}&issuer={issuerEncoded}&algorithm=SHA1&digits=6&period=30";
        }

        private string FormatManualEntryKey(string secret)
        {
            // Format as groups of 4 characters for easier manual entry
            // Example: JBSW Y3DP EHPK 3PXP
            var formatted = string.Empty;
            for (int i = 0; i < secret.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                {
                    formatted += " ";
                }
                formatted += secret[i];
            }
            return formatted;
        }
    }
}