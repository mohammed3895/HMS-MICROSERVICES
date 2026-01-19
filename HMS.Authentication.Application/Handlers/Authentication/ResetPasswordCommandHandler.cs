using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<object>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            IEmailService emailService,
            IAuditService auditService,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate passwords match
                if (request.NewPassword != request.ConfirmPassword)
                {
                    return Result<object>.Failure("Passwords do not match");
                }

                // Find user by email
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset attempt for non-existent email: {Email}", request.Email);
                    return Result<object>.Failure("Invalid reset token or email");
                }

                // Hash the provided token
                var tokenHash = ComputeSha256Hash(request.Token);

                // Find and validate the reset token
                var resetToken = await _context.PasswordResetTokens
                    .Where(t => t.UserId == user.Id && t.Token == tokenHash && !t.IsUsed)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (resetToken == null)
                {
                    _logger.LogWarning("Invalid password reset token for user: {Email}", request.Email);
                    return Result<object>.Failure("Invalid or expired reset token");
                }

                if (resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Expired password reset token for user: {Email}", request.Email);
                    return Result<object>.Failure("Reset token has expired. Please request a new one.");
                }

                // Mark token as used
                resetToken.IsUsed = true;
                resetToken.UsedAt = DateTime.UtcNow;

                // Reset the password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password reset failed for user {Email}: {Errors}", request.Email, errors);
                    return Result<object>.Failure($"Password reset failed: {errors}");
                }

                // Invalidate all refresh tokens for security
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);

                // Revoke all active sessions
                var sessions = await _context.UserSessions
                    .Where(s => s.UserId == user.Id && s.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var session in sessions)
                {
                    session.IsActive = false;
                    session.IsRevoked = true;
                    session.RevokedAt = DateTime.UtcNow;
                    session.RevokeReason = "Password was reset";
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Send confirmation email
                await _emailService.SendPasswordChangedEmailAsync(user);

                await _auditService.LogAsync("PasswordReset", "Security", user.Id.ToString(),
                    "Password was reset successfully", null);

                _logger.LogInformation("Password reset successfully for user: {Email}", request.Email);

                return Result<object>.Success(new
                {
                    message = "Password has been reset successfully. You can now log in with your new password."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for: {Email}", request.Email);
                return Result<object>.Failure("An error occurred while resetting your password");
            }
        }

        private string ComputeSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

