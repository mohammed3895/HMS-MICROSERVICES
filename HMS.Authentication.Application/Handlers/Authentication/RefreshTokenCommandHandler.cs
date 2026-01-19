using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return Result<LoginResponse>.Failure("User not found");
                }

                if (user.RefreshToken != request.RefreshToken)
                {
                    _logger.LogWarning("Invalid refresh token for user: {UserId}", user.Id);
                    return Result<LoginResponse>.Failure("Invalid refresh token");
                }

                if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Expired refresh token for user: {UserId}", user.Id);
                    return Result<LoginResponse>.Failure("Refresh token has expired");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var (newAccessToken, newRefreshToken) = await _tokenService.RefreshTokensAsync(
                    user, request.RefreshToken, roles.ToList(), request.DeviceId, null);

                // Update user with new refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Tokens refreshed successfully for user: {UserId}", user.Id);

                return Result<LoginResponse>.Success(new LoginResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
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
                _logger.LogError(ex, "Error refreshing tokens for user: {UserId}", request.UserId);
                return Result<LoginResponse>.Failure("An error occurred while refreshing tokens");
            }
        }
    }
}
