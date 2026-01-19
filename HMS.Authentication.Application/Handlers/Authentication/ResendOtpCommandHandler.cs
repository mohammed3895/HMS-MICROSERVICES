using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, Result<object>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ResendOtpCommandHandler> _logger;

        public ResendOtpCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService,
            IEmailService emailService,
            ILogger<ResendOtpCommandHandler> logger)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // Don't reveal that user doesn't exist
                    _logger.LogWarning("OTP resend attempt for non-existent email: {Email}", request.Email);
                    return Result<object>.Success(new { message = "If an account exists, a verification code has been sent." });
                }

                // Use the ResendOtpAsync method which includes rate limiting
                var success = await _otpService.ResendOtpAsync(user.Id, _emailService, user, null);

                if (!success)
                {
                    return Result<object>.Failure("Too many resend requests. Please try again later.");
                }

                _logger.LogInformation("OTP resent to: {Email}", request.Email);

                return Result<object>.Success(new
                {
                    message = "A new verification code has been sent to your email."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Rate limit exceeded for OTP resend: {Email}", request.Email);
                return Result<object>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP to: {Email}", request.Email);
                return Result<object>.Failure("An error occurred while resending the verification code");
            }
        }
    }
}