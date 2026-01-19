using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<object>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;

        public ConfirmEmailCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService,
            IEmailService emailService,
            IAuditService auditService,
            ILogger<ConfirmEmailCommandHandler> logger)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("Email confirmation attempt for non-existent email: {Email}", request.Email);
                    return Result<object>.Failure("User not found");
                }

                if (user.EmailConfirmed)
                {
                    return Result<object>.Failure("Email is already confirmed");
                }

                // Verify OTP
                var isValidOtp = await _otpService.VerifyOtpAsync(user.Id, request.OtpCode, "registration", null);
                if (!isValidOtp)
                {
                    _logger.LogWarning("Invalid OTP for email confirmation: {Email}", request.Email);
                    return Result<object>.Failure("Invalid or expired verification code");
                }

                // Confirm email
                user.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return Result<object>.Failure($"Failed to confirm email: {errors}");
                }

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user);

                await _auditService.LogAsync("EmailConfirmed", "User", user.Id.ToString(),
                    $"Email confirmed: {user.Email}", null);

                _logger.LogInformation("Email confirmed successfully: {UserId}", user.Id);

                return Result<object>.Success(new
                {
                    message = "Email confirmed successfully! You can now log in to your account.",
                    userId = user.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for: {Email}", request.Email);
                return Result<object>.Failure("An error occurred during email confirmation");
            }
        }
    }
}