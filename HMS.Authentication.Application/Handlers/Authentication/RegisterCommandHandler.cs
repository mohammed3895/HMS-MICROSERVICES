using HMS.Authentication.Application.Commands.Authentication;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Authentication
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly ILogger<RegisterCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IAuditService _auditService;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            ILogger<RegisterCommandHandler> logger,
            IEmailService emailService,
            IOtpService otpService,
            IAuditService auditService)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _otpService = otpService;
            _auditService = auditService;
        }

        public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Patient registration attempt: {Email}", request.Email);

                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
                    return Result<RegisterResponse>.Failure("A user with this email already exists");
                }

                // Create user
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    NationalId = request.NationalId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false // Will be confirmed via OTP
                };

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("User creation failed: {Errors}", errors);
                    return Result<RegisterResponse>.Failure($"Registration failed: {errors}");
                }

                // Assign Patient role
                await _userManager.AddToRoleAsync(user, "Patient");

                // Create patient profile
                var profile = new UserProfile
                {
                    UserId = user.Id,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    ZipCode = request.PostalCode,
                    BloodGroup = request.BloodGroup,
                    EmergencyContactName = request.EmergencyContactName,
                    EmergencyContactPhone = request.EmergencyContactPhone,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync(cancellationToken);

                // Generate and send OTP
                var otpCode = await _otpService.GenerateOtpAsync(user.Id, "registration", null);
                await _emailService.SendOtpEmailAsync(user, otpCode);

                await _auditService.LogAsync("PatientRegistered", "User", user.Id.ToString(),
                    $"New patient registered: {user.Email}", null);

                _logger.LogInformation("Patient registered successfully: {UserId}", user.Id);

                return Result<RegisterResponse>.Success(new RegisterResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailConfirmationRequired = true,
                    Message = "Registration successful! Please check your email for the verification code."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during patient registration");
                return Result<RegisterResponse>.Failure("An error occurred during registration");
            }
        }
    }
}