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
    public class BeginWebAuthnRegistrationHandler
        : IRequestHandler<BeginWebAuthnRegistrationCommand, Result<WebAuthnRegistrationOptions>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebAuthnService _webAuthnService;
        private readonly ILogger<BeginWebAuthnRegistrationHandler> _logger;

        public BeginWebAuthnRegistrationHandler(
            AuthenticationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebAuthnService webAuthnService,
            ILogger<BeginWebAuthnRegistrationHandler> logger)
        {
            _context = context;
            _userManager = userManager;
            _webAuthnService = webAuthnService;
            _logger = logger;
        }

        public async Task<Result<WebAuthnRegistrationOptions>> Handle(
            BeginWebAuthnRegistrationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                {
                    return Result<WebAuthnRegistrationOptions>.Failure("User not found");
                }

                // Get existing credentials to exclude from registration
                var existingCredentials = await _context.UserCredentials
                    .Where(c => c.UserId == request.UserId && !c.IsRevoked)
                    .ToListAsync(cancellationToken);

                // Generate registration options using Fido2
                var credentialCreateOptions = await _webAuthnService.InitiateRegistrationAsync(
                    user,
                    existingCredentials);

                // Convert to DTO
                var options = new WebAuthnRegistrationOptions
                {
                    Challenge = Convert.ToBase64String(credentialCreateOptions.Challenge),
                    RpId = credentialCreateOptions.Rp.Id,
                    RpName = credentialCreateOptions.Rp.Name,
                    UserId = Convert.ToBase64String(credentialCreateOptions.User.Id),
                    UserName = credentialCreateOptions.User.Name,
                    UserDisplayName = credentialCreateOptions.User.DisplayName,
                    Timeout = (int)credentialCreateOptions.Timeout,
                    Attestation = credentialCreateOptions.Attestation.ToString().ToLower() ?? "none"
                };

                _logger.LogInformation("WebAuthn registration initiated for user {UserId}", user.Id);

                return Result<WebAuthnRegistrationOptions>.Success(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error beginning WebAuthn registration for user {UserId}",
                    request.UserId);
                return Result<WebAuthnRegistrationOptions>.Failure(
                    "An error occurred while initiating registration");
            }
        }
    }
}