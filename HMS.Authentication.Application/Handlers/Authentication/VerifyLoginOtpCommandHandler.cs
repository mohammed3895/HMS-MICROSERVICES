using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class VerifyLoginOtpCommandHandler : IRequestHandler<VerifyLoginOtpCommand, Result<LoginResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly IOtpService _otpService;
        private readonly ITokenService _tokenService;
        private readonly IDeviceService _deviceService;
        private readonly IAuditService _auditService;
        private readonly ILogger<VerifyLoginOtpCommandHandler> _logger;

        public VerifyLoginOtpCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            IOtpService otpService,
            ITokenService tokenService,
            IDeviceService deviceService,
            IAuditService auditService,
            ILogger<VerifyLoginOtpCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _otpService = otpService;
            _tokenService = tokenService;
            _deviceService = deviceService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Handle(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return Result<LoginResponse>.Failure("User not found");
                }

                // Verify 2FA token if provided
                if (!string.IsNullOrEmpty(request.TwoFactorToken))
                {
                    var isValidToken = await _tokenService.Validate2FATokenAsync(request.UserId, request.TwoFactorToken);
                    if (!isValidToken)
                    {
                        return Result<LoginResponse>.Failure("Invalid or expired session");
                    }
                }

                // Verify OTP
                var isValidOtp = await _otpService.VerifyOtpAsync(user.Id, request.OtpCode, "login", null);
                if (!isValidOtp)
                {
                    _logger.LogWarning("Invalid OTP for login: {UserId}", user.Id);
                    return Result<LoginResponse>.Failure("Invalid or expired verification code");
                }

                // Trust device if requested
                if (request.TrustDevice && !string.IsNullOrEmpty(request.DeviceId))
                {
                    await _deviceService.TrustDeviceAsync(user.Id, request.DeviceId);
                    _logger.LogInformation("Device trusted for user: {UserId}", user.Id);
                }

                // Generate tokens
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = await _tokenService.GenerateAccessTokenAsync(user, roles.ToList(), request.DeviceId, null);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Update user
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Register/update device
                if (!string.IsNullOrEmpty(request.DeviceId))
                {
                    await _deviceService.RegisterOrUpdateDeviceAsync(user.Id, request.DeviceId);
                }

                // Record login
                var loginHistory = new LoginHistory
                {
                    UserId = user.Id,
                    LoginTime = DateTime.UtcNow,
                    IsSuccess = true,
                    LoginMethod = "OTP",
                    WasOtpUsed = true,
                    IpAddress = "0.0.0.0", // Get from HttpContext
                    UserAgent = "Unknown"
                };
                _context.LoginHistories.Add(loginHistory);
                await _context.SaveChangesAsync(cancellationToken);

                await _auditService.LogAsync("LoginWithOTP", "Authentication", user.Id.ToString(),
                    "User logged in successfully using OTP", null);

                _logger.LogInformation("User logged in with OTP: {UserId}", user.Id);

                return Result<LoginResponse>.Success(new LoginResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    UserInfo = new UserInfoDto
                    {
                        UserId = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Roles = roles.ToList(),
                        ProfilePictureUrl = user.ProfilePictureUrl,
                        IsTwoFactorEnabled = user.IsTwoFactorEnabled,
                        IsWebAuthnEnabled = user.IsWebAuthnEnabled
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying login OTP for user: {UserId}", request.UserId);
                return Result<LoginResponse>.Failure("An error occurred during OTP verification");
            }
        }
    }
}
