using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<object>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAuditService _auditService;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            ITokenService tokenService,
            IAuditService auditService,
            ILogger<LogoutCommandHandler> logger)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return Result<object>.Failure("User not found");
                }

                if (request.LogoutAllDevices)
                {
                    // Revoke all user tokens
                    await _tokenService.RevokeAllUserTokensAsync(user.Id);

                    // Revoke all sessions
                    var sessions = await _context.UserSessions
                        .Where(s => s.UserId == user.Id && s.IsActive)
                        .ToListAsync(cancellationToken);

                    foreach (var session in sessions)
                    {
                        session.IsActive = false;
                        session.IsRevoked = true;
                        session.RevokedAt = DateTime.UtcNow;
                        session.RevokeReason = "User logged out from all devices";
                    }

                    await _context.SaveChangesAsync(cancellationToken);

                    await _auditService.LogAsync("LogoutAllDevices", "Authentication", user.Id.ToString(),
                        "User logged out from all devices", null);

                    _logger.LogInformation("User logged out from all devices: {UserId}", user.Id);
                }
                else
                {
                    // Just clear refresh token
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    await _userManager.UpdateAsync(user);

                    await _auditService.LogAsync("Logout", "Authentication", user.Id.ToString(),
                        "User logged out", null);

                    _logger.LogInformation("User logged out: {UserId}", user.Id);
                }

                return Result<object>.Success(new
                {
                    message = request.LogoutAllDevices ?
                        "Successfully logged out from all devices" :
                        "Successfully logged out"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserId}", request.UserId);
                return Result<object>.Failure("An error occurred during logout");
            }
        }
    }
}
