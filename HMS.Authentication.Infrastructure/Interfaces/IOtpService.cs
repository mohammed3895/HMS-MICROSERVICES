using HMS.Authentication.Domain.Entities;

namespace HMS.Authentication.Infrastructure.Interfaces
{
    public interface IOtpService
    {
        /// <summary>
        /// Generate and store OTP for a user
        /// </summary>
        Task<string> GenerateOtpAsync(Guid userId, string purpose = "login", string? ipAddress = null);

        /// <summary>
        /// Verify OTP for a user
        /// </summary>
        Task<bool> VerifyOtpAsync(Guid userId, string otpCode, string purpose = "login", string? ipAddress = null);

        /// <summary>
        /// Resend OTP to user's email
        /// </summary>
        Task<bool> ResendOtpAsync(Guid userId, IEmailService emailService, ApplicationUser user, string? ipAddress = null);

        /// <summary>
        /// Clear OTP after successful verification or timeout
        /// </summary>
        Task ClearOtpAsync(Guid userId, string purpose = "login");

        /// <summary>
        /// Check if OTP is required for a user
        /// </summary>
        Task<bool> IsOtpRequiredAsync(Guid userId, string purpose = "login");
    }
}
