using HMS.Web.Interfaces;
using HMS.Web.Models.DTOs.Auth;
using HMS.Web.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

public class AuthService : IAuthService
{
    private readonly Lazy<IApiClientService> _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
           Lazy<IApiClientService> apiClient,
           IHttpContextAccessor httpContextAccessor,
           ILogger<AuthService> logger)
    {
        _apiClient = apiClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    // ==================== Authentication ====================

    public async Task<AuthResponseDto> LoginAsync(LoginViewModel model)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", model.Email);

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/login",
                new
                {
                    model.Email,
                    model.Password,
                    model.DeviceId,
                    model.OtpCode
                });

            if (response.IsSuccess && response.Data != null)
            {
                _logger.LogInformation("Login successful for email: {Email}", model.Email);

                if (!response.Data.RequiresTwoFactor &&
                    !response.Data.RequiresEmailVerification &&
                    !string.IsNullOrEmpty(response.Data.AccessToken))
                {
                    await SetTokensAsync(response.Data);
                }
            }
            else
            {
                _logger.LogWarning("Login failed for email: {Email}. Message: {Message}",
                    model.Email, response.Message);
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during login for email: {Email}", model.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Unable to connect to authentication server. Please try again."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for email: {Email}", model.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An unexpected error occurred during login. Please try again."
            };
        }
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterViewModel model)
    {
        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}", model.Email);

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/register",
                new
                {
                    model.Email,
                    model.Password,
                    model.ConfirmPassword,
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber,
                    model.DateOfBirth,
                    model.NationalId,
                    model.Address,
                    model.City,
                    model.State,
                    model.Country,
                    model.PostalCode
                });

            if (response.IsSuccess)
            {
                _logger.LogInformation("Registration successful for email: {Email}", model.Email);
            }
            else
            {
                _logger.LogWarning("Registration failed for email: {Email}. Message: {Message}",
                    model.Email, response.Message);
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during registration for email: {Email}", model.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Unable to connect to authentication server."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error for email: {Email}", model.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred during registration. Please try again."
            };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                try
                {
                    await _apiClient.Value.PostAsync<AuthResponseDto>(
                        "/auth/logout",
                        new { UserId = userId.Value });

                    _logger.LogInformation("Logout API call successful for user: {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "API logout call failed for user: {UserId}. Continuing with local logout", userId);
                }
            }

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Session.Clear();
                _logger.LogInformation("Local logout completed for user: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout error");
            throw;
        }
    }

    // ==================== Email Verification ====================

    public async Task<AuthResponseDto> ConfirmEmailAsync(VerifyEmailViewModel model)
    {
        try
        {
            _logger.LogInformation("Email confirmation attempt for user: {UserId}", model.UserId);

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/confirm-email",
                new
                {
                    model.UserId,
                    Token = model.Token ?? string.Empty,
                    model.OtpCode
                });

            if (response.IsSuccess)
            {
                _logger.LogInformation("Email confirmed successfully for user: {UserId}", model.UserId);
            }
            else
            {
                _logger.LogWarning("Email confirmation failed for user: {UserId}. Message: {Message}",
                    model.UserId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email confirmation error for user: {UserId}", model.UserId);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred during email verification. Please try again."
            };
        }
    }

    public async Task<AuthResponseDto> ResendOtpAsync(string email)
    {
        try
        {
            _logger.LogInformation("Resend OTP attempt for email: {Email}", email);

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/resend-otp",
                new { Email = email });

            if (response.IsSuccess)
            {
                _logger.LogInformation("OTP resent successfully to email: {Email}", email);
            }
            else
            {
                _logger.LogWarning("OTP resend failed for email: {Email}. Message: {Message}",
                    email, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resend OTP error for email: {Email}", email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred while resending the verification code."
            };
        }
    }

    // ==================== Two-Factor Authentication ====================

    public async Task<AuthResponseDto> VerifyOtpAsync(VerifyTwoFactorViewModel model)
    {
        try
        {
            _logger.LogInformation("OTP verification attempt for user: {UserId}", model.UserId);

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/verify-otp",
                new
                {
                    model.UserId,
                    model.OtpCode
                });

            if (response.IsSuccess && response.Data != null && !string.IsNullOrEmpty(response.Data.AccessToken))
            {
                await SetTokensAsync(response.Data);
                _logger.LogInformation("OTP verified successfully for user: {UserId}", model.UserId);
            }
            else
            {
                _logger.LogWarning("OTP verification failed for user: {UserId}. Message: {Message}",
                    model.UserId, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OTP verification error for user: {UserId}", model.UserId);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred during verification."
            };
        }
    }

    // ==================== Password Reset ====================

    public async Task<AuthResponseDto> RequestPasswordResetAsync(ForgotPasswordViewModel model)
    {
        try
        {
            _logger.LogInformation("Password reset request for email: {Email}", model.Email);

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/request-password-reset",
                new { model.Email });

            if (response.IsSuccess)
            {
                _logger.LogInformation("Password reset email sent to: {Email}", model.Email);
            }
            else
            {
                _logger.LogWarning("Password reset request failed for email: {Email}. Message: {Message}",
                    model.Email, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset request error for email: {Email}", model.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred. Please try again."
            };
        }
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        try
        {
            _logger.LogInformation("Password reset attempt for email: {Email}", model.Email);

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/reset-password",
                new
                {
                    model.Email,
                    model.Token,
                    NewPassword = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                });

            if (response.IsSuccess)
            {
                _logger.LogInformation("Password reset successfully for email: {Email}", model.Email);
            }
            else
            {
                _logger.LogWarning("Password reset failed for email: {Email}. Message: {Message}",
                    model.Email, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset error for email: {Email}", model.Email);
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "An error occurred during password reset."
            };
        }
    }

    // ==================== Token Management ====================

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            var token = context?.Session.GetString("access_token") ?? string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Access token not found in session");
            }

            return await Task.FromResult(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access token");
            return string.Empty;
        }
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            var token = context?.Session.GetString("refresh_token") ?? string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Refresh token not found in session");
            }

            return await Task.FromResult(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token");
            return string.Empty;
        }
    }

    public async Task SetTokensAsync(AuthDataDto data)
    {
        if (data == null)
        {
            _logger.LogWarning("Attempted to set null auth data");
            return;
        }

        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            _logger.LogWarning("HttpContext is null, cannot set tokens");
            return;
        }

        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()),
                new Claim(ClaimTypes.Email, data.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{data.FirstName} {data.LastName}".Trim())
            };

            if (!string.IsNullOrEmpty(data.PhoneNumber))
            {
                claims.Add(new Claim(ClaimTypes.MobilePhone, data.PhoneNumber));
            }

            if (data.Roles != null && data.Roles.Any())
            {
                foreach (var role in data.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = data.ExpiresAt,
                IsPersistent = true,
                AllowRefresh = true
            };

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            context.Session.SetString("access_token", data.AccessToken ?? string.Empty);
            context.Session.SetString("refresh_token", data.RefreshToken ?? string.Empty);
            context.Session.SetString("user_id", data.UserId.ToString());
            context.Session.SetString("user_email", data.Email ?? string.Empty);
            context.Session.SetString("user_name", $"{data.FirstName} {data.LastName}".Trim());

            _logger.LogInformation("Tokens set successfully for user: {UserId}", data.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting tokens for user: {UserId}", data.UserId);
            throw;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await GetRefreshTokenAsync();
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(refreshToken) || !userId.HasValue)
            {
                _logger.LogWarning("Cannot refresh token: missing refresh token or user ID");
                return false;
            }

            var response = await _apiClient.Value.PostAsync<AuthResponseDto>(
                "/auth/refresh-token",
                new
                {
                    UserId = userId.Value,
                    RefreshToken = refreshToken
                });

            if (response.IsSuccess && response.Data != null)
            {
                await SetTokensAsync(response.Data);
                _logger.LogInformation("Token refreshed successfully for user: {UserId}", userId);
                return true;
            }

            _logger.LogWarning("Token refresh failed for user: {UserId}. Message: {Message}",
                userId, response.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return false;
        }
    }

    // ==================== User State ====================

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            var isAuthenticated = context?.User?.Identity?.IsAuthenticated ?? false;
            return await Task.FromResult(isAuthenticated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authentication status");
            return false;
        }
    }

    public Guid? GetCurrentUserId()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            var userIdClaim = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("User ID claim not found");
                return null;
            }

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            _logger.LogWarning("Invalid user ID format: {UserId}", userIdClaim);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user ID");
            return null;
        }
    }

    public string GetCurrentUserEmail()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            var email = context?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("User email claim not found");
            }

            return email;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user email");
            return string.Empty;
        }
    }

    public string GetCurrentUserName()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            var name = context?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("User name claim not found");
            }

            return name;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user name");
            return string.Empty;
        }
    }

    // ==================== Helper Methods ====================

    public Task<bool> ValidatePasswordStrength(string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                _logger.LogWarning("Password validation failed: length < 8");
                return Task.FromResult(false);
            }

            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasDigits = password.Any(char.IsDigit);
            var hasSpecialChars = password.Any(ch => !char.IsLetterOrDigit(ch));

            var isStrong = hasUpperCase && hasLowerCase && hasDigits && hasSpecialChars;

            if (!isStrong)
            {
                _logger.LogWarning("Password validation failed: missing complexity requirements");
                _logger.LogDebug("Password requirements: Upper={HasUpper}, Lower={HasLower}, Digits={HasDigits}, Special={HasSpecial}",
                    hasUpperCase, hasLowerCase, hasDigits, hasSpecialChars);
            }

            return Task.FromResult(isStrong);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating password strength");
            return Task.FromResult(false);
        }
    }
}