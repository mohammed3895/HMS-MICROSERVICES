using HMS.Web.Models.DTOs.Auth;
using HMS.Web.Models.ViewModels.Auth;

namespace HMS.Web.Interfaces
{
    public interface IAuthService
    {
        // Authentication
        Task<AuthResponseDto> LoginAsync(LoginViewModel model);
        Task<AuthResponseDto> RegisterAsync(RegisterViewModel model);
        Task LogoutAsync();

        // Email Verification
        Task<AuthResponseDto> ConfirmEmailAsync(VerifyEmailViewModel model);
        Task<AuthResponseDto> ResendOtpAsync(string email);

        // Two-Factor Authentication
        Task<AuthResponseDto> VerifyOtpAsync(VerifyTwoFactorViewModel model);

        // Password Reset
        Task<AuthResponseDto> RequestPasswordResetAsync(ForgotPasswordViewModel model);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordViewModel model);

        // Token Management
        Task<string> GetAccessTokenAsync();
        Task<string> GetRefreshTokenAsync();
        Task SetTokensAsync(AuthDataDto data);
        Task<bool> RefreshTokenAsync();

        // User State
        Task<bool> IsAuthenticatedAsync();
        Guid? GetCurrentUserId();
        string GetCurrentUserEmail();
        string GetCurrentUserName();

        // Helper Methods
        Task<bool> ValidatePasswordStrength(string password);
    }
}