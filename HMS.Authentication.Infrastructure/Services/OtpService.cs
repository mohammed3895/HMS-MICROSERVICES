using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace HMS.Authentication.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OtpService> _logger;
        private readonly IAuditService _auditService;

        private const int MAX_OTP_ATTEMPTS = 3;
        private const int LOCKOUT_MINUTES = 15;
        private const int OTP_LENGTH = 6;

        public OtpService(
            ICacheService cacheService,
            IConfiguration configuration,
            ILogger<OtpService> logger,
            IAuditService auditService)
        {
            _cacheService = cacheService;
            _configuration = configuration;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task<string> GenerateOtpAsync(
            Guid userId,
            string purpose = "login",
            string? ipAddress = null)
        {
            try
            {
                // Check if user is locked out
                if (await IsUserLockedOutAsync(userId, purpose))
                {
                    _logger.LogWarning("OTP generation blocked - user {UserId} is locked out", userId);
                    throw new InvalidOperationException("Too many attempts. Please try again later.");
                }

                // Generate cryptographically secure OTP
                var otpCode = GenerateSecureOtp();
                var nonce = GenerateNonce();

                var otpData = new OtpData
                {
                    Code = otpCode,
                    Nonce = nonce,
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        purpose == "login" ? 5 : 10), // 5 min for login, 10 for registration
                    Attempts = 0
                };

                var cacheKey = GetOtpCacheKey(userId, purpose);

                await _cacheService.SetAsync(
                    cacheKey,
                    otpData,
                    TimeSpan.FromMinutes(purpose == "login" ? 5 : 10));

                await _auditService.LogAsync(
                    "OtpGenerated",
                    "Authentication",
                    userId.ToString(),
                    $"OTP generated for purpose: {purpose}, IP: {ipAddress}",
                    null);

                _logger.LogInformation(
                    "OTP generated for user {UserId} with purpose: {Purpose}",
                    userId, purpose);

                return otpCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> VerifyOtpAsync(
            Guid userId,
            string otpCode,
            string purpose = "login",
            string? ipAddress = null)
        {
            try
            {
                var cacheKey = GetOtpCacheKey(userId, purpose);
                var otpData = await _cacheService.GetAsync<OtpData>(cacheKey);

                if (otpData == null)
                {
                    _logger.LogWarning("No OTP found for user {UserId}", userId);
                    await RecordFailedAttemptAsync(userId, purpose, "OTP not found");
                    return false;
                }

                // Check expiration
                if (DateTime.UtcNow > otpData.ExpiresAt)
                {
                    _logger.LogWarning("Expired OTP provided for user {UserId}", userId);
                    await _cacheService.RemoveAsync(cacheKey);
                    await RecordFailedAttemptAsync(userId, purpose, "OTP expired");
                    return false;
                }

                // Check IP binding (for login OTPs only)
                if (purpose == "login" && !string.IsNullOrEmpty(otpData.IpAddress))
                {
                    if (otpData.IpAddress != ipAddress)
                    {
                        _logger.LogWarning(
                            "IP mismatch for OTP verification. Expected: {Expected}, Got: {Actual}",
                            otpData.IpAddress, ipAddress);
                        await RecordFailedAttemptAsync(userId, purpose, "IP mismatch");
                        return false;
                    }
                }

                // Check attempts
                otpData.Attempts++;
                if (otpData.Attempts > MAX_OTP_ATTEMPTS)
                {
                    _logger.LogWarning(
                        "Max OTP attempts exceeded for user {UserId}", userId);
                    await _cacheService.RemoveAsync(cacheKey);
                    await LockoutUserAsync(userId, purpose);
                    await RecordFailedAttemptAsync(userId, purpose, "Max attempts exceeded");
                    return false;
                }

                // Verify OTP using constant-time comparison
                if (!ConstantTimeEquals(otpData.Code, otpCode))
                {
                    _logger.LogWarning("Invalid OTP provided for user {UserId}", userId);

                    // Update attempts count
                    await _cacheService.SetAsync(
                        cacheKey,
                        otpData,
                        otpData.ExpiresAt - DateTime.UtcNow);

                    await RecordFailedAttemptAsync(userId, purpose, "Invalid OTP");
                    return false;
                }

                // OTP verified successfully - remove it (single use)
                await _cacheService.RemoveAsync(cacheKey);
                await ClearFailedAttemptsAsync(userId, purpose);

                await _auditService.LogAsync(
                    "OtpVerified",
                    "Authentication",
                    userId.ToString(),
                    $"OTP verified successfully for purpose: {purpose}",
                    null);

                _logger.LogInformation(
                    "OTP verified successfully for user {UserId}", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ResendOtpAsync(
            Guid userId,
            IEmailService emailService,
            ApplicationUser user,
            string? ipAddress = null)
        {
            try
            {
                // Rate limit resend requests (max 3 per hour)
                var resendKey = $"otp:resend:{userId}";
                var resendCount = await _cacheService.GetAsync<int>(resendKey);

                if (resendCount >= 3)
                {
                    _logger.LogWarning(
                        "Resend rate limit exceeded for user {UserId}", userId);
                    throw new InvalidOperationException(
                        "Too many resend requests. Please try again later.");
                }

                // Generate new OTP (use "registration" purpose for email confirmation)
                var otpCode = await GenerateOtpAsync(userId, "registration", ipAddress);
                await emailService.SendOtpEmailAsync(user, otpCode);

                // Increment resend counter
                await _cacheService.SetAsync(
                    resendKey,
                    resendCount + 1,
                    TimeSpan.FromHours(1));

                await _auditService.LogAsync(
                    "OtpResent",
                    "Authentication",
                    userId.ToString(),
                    $"OTP resent to user",
                    null);

                _logger.LogInformation("OTP resent to user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP to user {UserId}", userId);
                return false;
            }
        }

        public async Task ClearOtpAsync(Guid userId, string purpose = "login")
        {
            try
            {
                var cacheKey = GetOtpCacheKey(userId, purpose);
                await _cacheService.RemoveAsync(cacheKey);
                _logger.LogInformation("OTP cleared for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing OTP for user {UserId}", userId);
            }
        }

        public async Task<bool> IsOtpRequiredAsync(Guid userId, string purpose = "login")
        {
            var cacheKey = GetOtpCacheKey(userId, purpose);
            return await _cacheService.ExistsAsync(cacheKey);
        }

        private async Task<bool> IsUserLockedOutAsync(Guid userId, string purpose)
        {
            var lockoutKey = GetLockoutCacheKey(userId, purpose);
            return await _cacheService.ExistsAsync(lockoutKey);
        }

        private async Task LockoutUserAsync(Guid userId, string purpose)
        {
            var lockoutKey = GetLockoutCacheKey(userId, purpose);
            await _cacheService.SetAsync(
                lockoutKey,
                new { LockedAt = DateTime.UtcNow },
                TimeSpan.FromMinutes(LOCKOUT_MINUTES));

            await _auditService.LogAsync(
                "OtpLockout",
                "Security",
                userId.ToString(),
                $"User locked out due to max OTP attempts for purpose: {purpose}",
                null);

            _logger.LogWarning(
                "User {UserId} locked out for {Minutes} minutes",
                userId, LOCKOUT_MINUTES);
        }

        private async Task RecordFailedAttemptAsync(
            Guid userId, string purpose, string reason)
        {
            await _auditService.LogAsync(
                "OtpVerificationFailed",
                "Security",
                userId.ToString(),
                $"OTP verification failed - Reason: {reason}, Purpose: {purpose}",
                null);
        }

        private async Task ClearFailedAttemptsAsync(Guid userId, string purpose)
        {
            var resendKey = $"otp:resend:{userId}";
            await _cacheService.RemoveAsync(resendKey);
        }

        private string GenerateSecureOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var number = BitConverter.ToUInt32(bytes, 0) % 1000000;
            return number.ToString($"D{OTP_LENGTH}");
        }

        private string GenerateNonce()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private bool ConstantTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var result = 0;
            for (var i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        private string GetOtpCacheKey(Guid userId, string purpose)
            => $"otp:{userId}:{purpose}";

        private string GetLockoutCacheKey(Guid userId, string purpose)
            => $"otp:lockout:{userId}:{purpose}";

        private class OtpData
        {
            public string Code { get; set; } = string.Empty;
            public string Nonce { get; set; } = string.Empty;
            public string? IpAddress { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public int Attempts { get; set; }
        }
    }
}