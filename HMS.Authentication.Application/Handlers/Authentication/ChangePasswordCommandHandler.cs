using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<object>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            IEmailService emailService,
            IAuditService auditService,
            ITokenService tokenService,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _auditService = auditService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate new passwords match
                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    return Result<object>.Failure("New passwords do not match");
                }

                // Find user
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Change password attempt for non-existent user: {UserId}", request.UserId);
                    return Result<object>.Failure("User not found");
                }

                // Verify current password
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    _logger.LogWarning("Invalid current password for user: {UserId}", request.UserId);

                    await _auditService.LogAsync("PasswordChangeFailedInvalidCurrent", "Security", user.Id.ToString(),
                        "Failed password change attempt - invalid current password", null);

                    return Result<object>.Failure("Current password is incorrect");
                }

                // Check if new password is same as current password
                var isSameAsOld = await _userManager.CheckPasswordAsync(user, request.NewPassword);
                if (isSameAsOld)
                {
                    return Result<object>.Failure("New password must be different from your current password");
                }

                // Change password
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password change failed for user {UserId}: {Errors}", request.UserId, errors);
                    return Result<object>.Failure($"Password change failed: {errors}");
                }

                // Invalidate all refresh tokens for security
                await _tokenService.RevokeAllUserTokensAsync(user.Id);

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);

                // Revoke all other sessions except current (optional - you may want to keep current session)
                var sessions = await _context.UserSessions
                    .Where(s => s.UserId == user.Id && s.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var session in sessions)
                {
                    session.IsActive = false;
                    session.IsRevoked = true;
                    session.RevokedAt = DateTime.UtcNow;
                    session.RevokeReason = "Password was changed";
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Send confirmation email
                await _emailService.SendPasswordChangedEmailAsync(user);

                await _auditService.LogAsync("PasswordChanged", "Security", user.Id.ToString(),
                    "Password changed successfully", null);

                _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

                return Result<object>.Success(new
                {
                    message = "Password changed successfully. Please log in again with your new password."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", request.UserId);
                return Result<object>.Failure("An error occurred while changing your password");
            }
        }
    }
}
