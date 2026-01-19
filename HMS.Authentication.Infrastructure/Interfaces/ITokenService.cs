
using HMS.Authentication.Domain.Entities;

namespace HMS.Authentication.Infrastructure.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(
            ApplicationUser user,
            List<string>? roles = null,
            string? deviceFingerprint = null,
            string? ipAddress = null);

        string GenerateRefreshToken();

        Task<string> GenerateRefreshTokenAsync(ApplicationUser user);

        Task<(string AccessToken, string RefreshToken)> RefreshTokensAsync(
            ApplicationUser user,
            string refreshToken,
            List<string>? roles = null,
            string? deviceFingerprint = null,
            string? ipAddress = null);

        Task<bool> ValidateTokenAsync(string token, string? deviceFingerprint = null);

        Task RevokeTokenAsync(string tokenOrHash, string reason);

        Task RevokeAllUserTokensAsync(Guid userId);

        Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user);

        Task<bool> Validate2FATokenAsync(Guid userId, string token);
    }
}