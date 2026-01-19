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
    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Result<object>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;
        private readonly ILogger<RequestPasswordResetCommandHandler> _logger;

        public RequestPasswordResetCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            IEmailService emailService,
            IAuditService auditService,
            ILogger<RequestPasswordResetCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                // Don't reveal whether email exists or not (prevent user enumeration)
                if (user == null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);

                    // Still return success to prevent email enumeration
                    return Result<object>.Success(new
                    {
                        message = "If an account with that email exists, a password reset link has been sent."
                    });
                }

                // Check if account is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Password reset requested for inactive account: {Email}", request.Email);

                    // Still return success to prevent account status enumeration
                    return Result<object>.Success(new
                    {
                        message = "If an account with that email exists, a password reset link has been sent."
                    });
                }

                // Invalidate any existing reset tokens for this user
                var existingTokens = await _context.PasswordResetTokens
                    .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync(cancellationToken);

                foreach (var token in existingTokens)
                {
                    token.IsUsed = true;
                }

                // Generate secure reset token
                var resetToken = GenerateSecureToken();
                var tokenHash = ComputeSha256Hash(resetToken);

                // Create password reset token entry
                var passwordResetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = tokenHash,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(passwordResetToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Send password reset email
                await _emailService.SendPasswordResetEmailAsync(user, resetToken);

                await _auditService.LogAsync("PasswordResetRequested", "Authentication", user.Id.ToString(),
                    "Password reset requested", null);

                _logger.LogInformation("Password reset email sent to: {Email}", request.Email);

                return Result<object>.Success(new
                {
                    message = "If an account with that email exists, a password reset link has been sent."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting password reset for: {Email}", request.Email);

                // Still return success to prevent information disclosure
                return Result<object>.Success(new
                {
                    message = "If an account with that email exists, a password reset link has been sent."
                });
            }
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
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
