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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuthenticationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAuditService _auditService;
        private readonly IDeviceService _deviceService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AuthenticationDbContext context,
            ITokenService tokenService,
            IAuditService auditService,
            IDeviceService deviceService,
            IOtpService otpService,
            IEmailService emailService,
            ILogger<LoginCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
            _auditService = auditService;
            _deviceService = deviceService;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    await RecordFailedLogin(null, request.Email, "User not found");
                    return Result<LoginResponse>.Failure("Invalid credentials");
                }

                // Check if email is confirmed
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Login attempt for unverified email: {Email}", request.Email);
                    return Result<LoginResponse>.Success(new LoginResponse
                    {
                        UserId = user.Id,
                        RequiresEmailVerification = true,
                        Message = "Please verify your email before logging in. Check your inbox for the verification code."
                    });
                }

                // Check if account is active
                if (!user.IsActive)
                {
                    await RecordFailedLogin(user.Id, request.Email, "Account deactivated");
                    return Result<LoginResponse>.Failure("Your account has been deactivated. Please contact support.");
                }

                // Check password
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

                if (result.IsLockedOut)
                {
                    await RecordFailedLogin(user.Id, request.Email, "Account locked");
                    await _auditService.LogAsync("AccountLockedOut", "Security", user.Id.ToString(),
                        "Account locked due to multiple failed login attempts", null);
                    return Result<LoginResponse>.Failure("Account is locked due to multiple failed login attempts. Please try again later.");
                }

                if (!result.Succeeded)
                {
                    await RecordFailedLogin(user.Id, request.Email, "Invalid password");
                    return Result<LoginResponse>.Failure("Invalid credentials");
                }

                // Check if device is trusted
                bool isDeviceTrusted = false;
                if (!string.IsNullOrEmpty(request.DeviceId))
                {
                    isDeviceTrusted = await _deviceService.IsDeviceTrustedAsync(user.Id, request.DeviceId);
                }

                // If 2FA is enabled, handle 2FA flow
                if (user.IsTwoFactorEnabled)
                {
                    var twoFactorToken = await _tokenService.GenerateTwoFactorTokenAsync(user);
                    return Result<LoginResponse>.Success(new LoginResponse
                    {
                        UserId = user.Id,
                        RequiresTwoFactor = true,
                        TwoFactorToken = twoFactorToken,
                        Message = "Please enter your two-factor authentication code."
                    });
                }

                // If device is not trusted, require OTP
                if (!isDeviceTrusted)
                {
                    var otpCode = await _otpService.GenerateOtpAsync(user.Id, "login", null);
                    await _emailService.SendOtpEmailAsync(user, otpCode);

                    var twoFactorToken = await _tokenService.GenerateTwoFactorTokenAsync(user);

                    _logger.LogInformation("OTP required for login: {UserId}", user.Id);

                    return Result<LoginResponse>.Success(new LoginResponse
                    {
                        UserId = user.Id,
                        RequiresOtp = true,
                        TwoFactorToken = twoFactorToken,
                        Message = "For your security, please enter the verification code sent to your email."
                    });
                }

                // Complete login
                return await CompleteLogin(user, request.DeviceId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return Result<LoginResponse>.Failure("An error occurred during login");
            }
        }

        private async Task<Result<LoginResponse>> CompleteLogin(ApplicationUser user, string? deviceId, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, roles.ToList(), deviceId, null);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Update user
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Register/update device
            if (!string.IsNullOrEmpty(deviceId))
            {
                await _deviceService.RegisterOrUpdateDeviceAsync(user.Id, deviceId);
            }

            // Record successful login
            await RecordSuccessfulLogin(user.Id, "Password+OTP");

            await _auditService.LogAsync("LoginSuccess", "Authentication", user.Id.ToString(),
                $"User logged in successfully", null);

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

        private async Task RecordFailedLogin(Guid? userId, string email, string reason)
        {
            var loginHistory = new LoginHistory
            {
                UserId = userId ?? Guid.Empty,
                LoginTime = DateTime.UtcNow,
                IsSuccess = false,
                FailureReason = reason,
                IpAddress = "0.0.0.0", // Get from HttpContext in real implementation
                UserAgent = "Unknown"
            };

            _context.LoginHistories.Add(loginHistory);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Failed login attempt: Email={Email}, Reason={Reason}", email, reason);
        }

        private async Task RecordSuccessfulLogin(Guid userId, string method)
        {
            var loginHistory = new LoginHistory
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IsSuccess = true,
                LoginMethod = method,
                IpAddress = "0.0.0.0", // Get from HttpContext
                UserAgent = "Unknown"
            };

            _context.LoginHistories.Add(loginHistory);
            await _context.SaveChangesAsync();
        }
    }
}