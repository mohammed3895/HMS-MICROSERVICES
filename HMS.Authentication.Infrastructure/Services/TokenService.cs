using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HMS.Authentication.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IAuditService _auditService;

        public TokenService(
            IConfiguration configuration,
            ILogger<TokenService> logger,
            ICacheService cacheService,
            IAuditService auditService)
        {
            _configuration = configuration;
            _logger = logger;
            _cacheService = cacheService;
            _auditService = auditService;
        }

        public async Task<string> GenerateAccessTokenAsync(
            ApplicationUser user,
            List<string>? roles = null,
            string? deviceFingerprint = null,
            string? ipAddress = null)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["Secret"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("JWT secret not configured");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

                var tokenId = Guid.NewGuid().ToString();
                var issuedAt = DateTime.UtcNow;
                var expiresAt = issuedAt.AddMinutes(
                    jwtSettings.GetValue<int>("AccessTokenExpiryMinutes", 15));

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString()),
                    new Claim("user_id", user.Id.ToString()),
                    new Claim("full_name", $"{user.FirstName} {user.LastName}"),
                };

                // Add roles
                if (roles != null && roles.Any())
                {
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                // Add device binding
                if (!string.IsNullOrEmpty(deviceFingerprint))
                {
                    var deviceHash = ComputeSha256Hash(deviceFingerprint);
                    claims.Add(new Claim("device_hash", deviceHash));
                }

                // Add IP binding (for high-security scenarios)
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    var ipHash = ComputeSha256Hash(ipAddress);
                    claims.Add(new Claim("ip_hash", ipHash));
                }

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    notBefore: issuedAt,
                    expires: expiresAt,
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Store token metadata for tracking
                await StoreTokenMetadataAsync(user.Id, tokenId, expiresAt, "access");

                await _auditService.LogAsync(
                    "AccessTokenGenerated",
                    "Authentication",
                    user.Id.ToString(),
                    $"Access token generated. Expires: {expiresAt}",
                    null);

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generating access token for user {UserId}", user.Id);
                throw;
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                using var rng = RandomNumberGenerator.Create();
                var randomBytes = new byte[64]; // 512 bits
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                throw;
            }
        }

        public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            try
            {
                // Generate cryptographically secure refresh token
                var refreshToken = GenerateRefreshToken();

                // Store refresh token metadata
                var tokenHash = ComputeSha256Hash(refreshToken);
                var expiresAt = DateTime.UtcNow.AddDays(7);

                await StoreRefreshTokenAsync(user.Id, tokenHash, expiresAt);

                await _auditService.LogAsync(
                    "RefreshTokenGenerated",
                    "Authentication",
                    user.Id.ToString(),
                    $"Refresh token generated. Expires: {expiresAt}",
                    null);

                _logger.LogInformation(
                    "Refresh token generated for user {UserId}", user.Id);

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generating refresh token for user {UserId}", user.Id);
                throw;
            }
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokensAsync(
            ApplicationUser user,
            string refreshToken,
            List<string>? roles = null,
            string? deviceFingerprint = null,
            string? ipAddress = null)
        {
            try
            {
                // Verify refresh token hasn't been used
                var tokenHash = ComputeSha256Hash(refreshToken);
                var isRevoked = await IsTokenRevokedAsync(tokenHash);

                if (isRevoked)
                {
                    _logger.LogWarning(
                        "Attempt to use revoked refresh token for user {UserId}", user.Id);

                    // Possible token theft - revoke all user sessions
                    await RevokeAllUserTokensAsync(user.Id);

                    await _auditService.LogAsync(
                        "SuspiciousTokenReuse",
                        "Security",
                        user.Id.ToString(),
                        "Revoked refresh token reused - all sessions terminated",
                        null);

                    throw new SecurityTokenException(
                        "Invalid refresh token. All sessions have been terminated for security.");
                }

                // Mark old refresh token as used (one-time use)
                await RevokeTokenAsync(tokenHash, "Token rotated");

                // Generate new tokens
                var newAccessToken = await GenerateAccessTokenAsync(
                    user, roles, deviceFingerprint, ipAddress);
                var newRefreshToken = await GenerateRefreshTokenAsync(user);

                await _auditService.LogAsync(
                    "TokensRefreshed",
                    "Authentication",
                    user.Id.ToString(),
                    "Access and refresh tokens refreshed",
                    null);

                return (newAccessToken, newRefreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error refreshing tokens for user {UserId}", user.Id);
                throw;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token, string? deviceFingerprint = null)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["Secret"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    return false;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
                };

                var principal = tokenHandler.ValidateToken(
                    token, validationParameters, out var validatedToken);

                // Check if token is in revocation list
                var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (!string.IsNullOrEmpty(jti) && await IsTokenRevokedAsync(jti))
                {
                    _logger.LogWarning("Revoked token used: {Jti}", jti);
                    return false;
                }

                // Verify device binding if provided
                if (!string.IsNullOrEmpty(deviceFingerprint))
                {
                    var deviceHash = ComputeSha256Hash(deviceFingerprint);
                    var tokenDeviceHash = principal.FindFirst("device_hash")?.Value;

                    if (!string.IsNullOrEmpty(tokenDeviceHash) &&
                        tokenDeviceHash != deviceHash)
                    {
                        _logger.LogWarning("Device fingerprint mismatch for token");
                        return false;
                    }
                }

                return true;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }

        public async Task RevokeTokenAsync(string tokenOrHash, string reason)
        {
            try
            {
                var cacheKey = $"revoked_token:{tokenOrHash}";
                await _cacheService.SetAsync(
                    cacheKey,
                    new { RevokedAt = DateTime.UtcNow, Reason = reason },
                    TimeSpan.FromDays(7)); // Keep for refresh token lifetime

                _logger.LogInformation(
                    "Token revoked: {Reason}", reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                throw;
            }
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            try
            {
                // Mark all user tokens as revoked
                var cacheKey = $"revoked_user:{userId}";
                await _cacheService.SetAsync(
                    cacheKey,
                    new { RevokedAt = DateTime.UtcNow },
                    TimeSpan.FromDays(7));

                await _auditService.LogAsync(
                    "AllTokensRevoked",
                    "Security",
                    userId.ToString(),
                    "All user tokens revoked",
                    null);

                _logger.LogWarning("All tokens revoked for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error revoking all tokens for user {UserId}", userId);
                throw;
            }
        }

        public async Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user)
        {
            try
            {
                // Generate time-limited 2FA session token
                var token = GenerateSecureToken(32);
                var cacheKey = $"2fa_token:{user.Id}";

                await _cacheService.SetAsync(
                    cacheKey,
                    token,
                    TimeSpan.FromMinutes(5)); // 5 minute expiry

                await _auditService.LogAsync(
                    "TwoFactorTokenGenerated",
                    "Authentication",
                    user.Id.ToString(),
                    "2FA token generated",
                    null);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generating 2FA token for user {UserId}", user.Id);
                throw;
            }
        }

        public async Task<bool> Validate2FATokenAsync(Guid userId, string token)
        {
            try
            {
                var cacheKey = $"2fa_token:{userId}";
                var storedToken = await _cacheService.GetAsync<string>(cacheKey);

                if (string.IsNullOrEmpty(storedToken))
                {
                    return false;
                }

                // Constant-time comparison
                var isValid = ConstantTimeEquals(storedToken, token);

                if (isValid)
                {
                    // Token used, remove it
                    await _cacheService.RemoveAsync(cacheKey);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error validating 2FA token for user {UserId}", userId);
                return false;
            }
        }

        private async Task<bool> IsTokenRevokedAsync(string tokenIdOrHash)
        {
            var directRevocation = await _cacheService.ExistsAsync(
                $"revoked_token:{tokenIdOrHash}");
            return directRevocation;
        }

        private async Task StoreTokenMetadataAsync(
            Guid userId,
            string tokenId,
            DateTime expiresAt,
            string type)
        {
            var cacheKey = $"token_meta:{tokenId}";
            await _cacheService.SetAsync(
                cacheKey,
                new
                {
                    UserId = userId,
                    Type = type,
                    ExpiresAt = expiresAt,
                    IssuedAt = DateTime.UtcNow
                },
                expiresAt - DateTime.UtcNow);
        }

        private async Task StoreRefreshTokenAsync(
            Guid userId,
            string tokenHash,
            DateTime expiresAt)
        {
            var cacheKey = $"refresh_token:{userId}:{tokenHash}";
            await _cacheService.SetAsync(
                cacheKey,
                new { IssuedAt = DateTime.UtcNow, ExpiresAt = expiresAt },
                expiresAt - DateTime.UtcNow);
        }

        private string ComputeSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string GenerateSecureToken(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
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
    }
}