using HMS.Authentication.Application.Commands.Authentication.WebAuth;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication.WebAuth
{
    public class BeginWebAuthnAuthenticationHandler
        : IRequestHandler<BeginWebAuthnAuthenticationCommand, Result<WebAuthnAuthenticationOptions>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebAuthnService _webAuthnService;
        private readonly ILogger<BeginWebAuthnAuthenticationHandler> _logger;

        public BeginWebAuthnAuthenticationHandler(
            AuthenticationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebAuthnService webAuthnService,
            ILogger<BeginWebAuthnAuthenticationHandler> logger)
        {
            _context = context;
            _userManager = userManager;
            _webAuthnService = webAuthnService;
            _logger = logger;
        }

        public async Task<Result<WebAuthnAuthenticationOptions>> Handle(
            BeginWebAuthnAuthenticationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("WebAuthn authentication attempt for non-existent email: {Email}",
                        request.Email);
                    return Result<WebAuthnAuthenticationOptions>.Failure("Invalid credentials");
                }

                // Get user's active WebAuthn credentials
                var credentials = await _context.UserCredentials
                    .Where(c => c.UserId == user.Id && !c.IsRevoked)
                    .ToListAsync(cancellationToken);

                if (!credentials.Any())
                {
                    return Result<WebAuthnAuthenticationOptions>.Failure(
                        "No WebAuthn credentials registered for this account");
                }

                // Generate authentication options using Fido2
                var assertionOptions = await _webAuthnService.InitiateAuthenticationAsync(user, credentials);

                // Convert to DTO
                var options = new WebAuthnAuthenticationOptions
                {
                    Challenge = Convert.ToBase64String(assertionOptions.Challenge),
                    RpId = assertionOptions.RpId,
                    Timeout = (int)assertionOptions.Timeout,
                    AllowCredentials = assertionOptions.AllowCredentials
                        .Select(c => Convert.ToBase64String(c.Id))
                        .ToList()
                };

                _logger.LogInformation("WebAuthn authentication initiated for user {UserId}", user.Id);

                return Result<WebAuthnAuthenticationOptions>.Success(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error beginning WebAuthn authentication for {Email}",
                    request.Email);
                return Result<WebAuthnAuthenticationOptions>.Failure(
                    "An error occurred while initiating authentication");
            }
        }
    }
}