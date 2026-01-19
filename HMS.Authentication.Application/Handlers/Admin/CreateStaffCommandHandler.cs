using HMS.Authentication.Application.Commands.Admin;
using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Admin
{
    public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Result<CreateStaffResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly ILogger<CreateStaffCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;

        public CreateStaffCommandHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            ILogger<CreateStaffCommandHandler> logger,
            IEmailService emailService,
            IAuditService auditService)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _auditService = auditService;
        }

        public async Task<Result<CreateStaffResponse>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Staff creation attempt by admin {AdminId}: Role={Role}, Email={Email}",
                    request.CreatedBy, request.Role, request.Email);

                // Validate role
                var validRoles = new[] { "Doctor", "Nurse", "Pharmacist", "LabTechnician", "Receptionist", "Admin", "Manager" };
                if (!validRoles.Contains(request.Role))
                {
                    return Result<CreateStaffResponse>.Failure($"Invalid role: {request.Role}");
                }

                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result<CreateStaffResponse>.Failure("A user with this email already exists");
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
                    EmailConfirmed = true // Staff accounts are pre-verified
                };

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return Result<CreateStaffResponse>.Failure($"Staff creation failed: {errors}");
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, request.Role);

                // Create role-specific profile
                await CreateRoleSpecificProfile(user.Id, request, cancellationToken);

                // Send welcome email with credentials
                await _emailService.SendWelcomeEmailAsync(user);

                await _auditService.LogAsync("StaffCreated", "User", user.Id.ToString(),
                    $"Staff member created by admin {request.CreatedBy}: Role={request.Role}", request.CreatedBy.ToString());

                _logger.LogInformation("Staff created successfully: {UserId}, Role={Role}", user.Id, request.Role);

                return Result<CreateStaffResponse>.Success(new CreateStaffResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = request.Role,
                    EmployeeId = request.EmployeeId,
                    TemporaryPassword = request.Password,
                    Message = $"{request.Role} account created successfully. Welcome email sent to {user.Email}."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during staff creation");
                return Result<CreateStaffResponse>.Failure("An error occurred during staff creation");
            }
        }

        private async Task CreateRoleSpecificProfile(Guid userId, CreateStaffCommand request, CancellationToken cancellationToken)
        {
            switch (request.Role)
            {
                case "Doctor":
                    var doctorProfile = new DoctorProfile
                    {
                        UserId = userId,
                        Department = request.Department,
                        Specialization = request.Specialization,
                        MedicalLicenseNumber = request.LicenseNumber ?? string.Empty,
                        LicenseExpiryDate = request.LicenseExpiryDate ?? DateTime.UtcNow.AddYears(5),
                        EmployeeId = request.EmployeeId,
                        Salary = request.Salary,
                        JoiningDate = request.JoiningDate,
                        Qualification = request.Qualification,
                        YearsOfExperience = request.YearsOfExperience,
                        Certifications = request.Certifications ?? new List<string>(),
                        ConsultationFee = request.ConsultationFee,
                        MaxPatientsPerDay = request.MaxPatientsPerDay ?? 20,
                        Biography = request.Biography
                    };
                    _context.DoctorProfiles.Add(doctorProfile);
                    break;

                case "Nurse":
                    var nurseProfile = new NurseProfile
                    {
                        UserId = userId,
                        Department = request.Department,
                        Specialization = request.Specialization,
                        NursingLicenseNumber = request.LicenseNumber ?? string.Empty,
                        LicenseExpiryDate = request.LicenseExpiryDate ?? DateTime.UtcNow.AddYears(5),
                        EmployeeId = request.EmployeeId,
                        Salary = request.Salary,
                        JoiningDate = request.JoiningDate,
                        YearsOfExperience = request.YearsOfExperience,
                        Shift = request.Shift ?? "Morning",
                        Ward = request.Ward
                    };
                    _context.NurseProfiles.Add(nurseProfile);
                    break;

                case "Pharmacist":
                    var pharmacistProfile = new PharmacistProfile
                    {
                        UserId = userId,
                        Department = request.Department,
                        PharmacyLicenseNumber = request.LicenseNumber ?? string.Empty,
                        LicenseExpiryDate = request.LicenseExpiryDate ?? DateTime.UtcNow.AddYears(5),
                        EmployeeId = request.EmployeeId,
                        Salary = request.Salary,
                        JoiningDate = request.JoiningDate,
                        YearsOfExperience = request.YearsOfExperience,
                        PharmacyLocation = request.PharmacyLocation
                    };
                    _context.PharmacistProfiles.Add(pharmacistProfile);
                    break;

                case "LabTechnician":
                    var labProfile = new LabTechnicianProfile
                    {
                        UserId = userId,
                        Department = request.Department,
                        EmployeeId = request.EmployeeId,
                        Salary = request.Salary,
                        JoiningDate = request.JoiningDate,
                        YearsOfExperience = request.YearsOfExperience,
                        LabSection = request.LabSection,
                        Certifications = request.Certifications ?? new List<string>()
                    };
                    _context.LabTechnicianProfiles.Add(labProfile);
                    break;

                case "Receptionist":
                    var receptionistProfile = new ReceptionistProfile
                    {
                        UserId = userId,
                        Department = request.Department,
                        EmployeeId = request.EmployeeId,
                        Salary = request.Salary,
                        JoiningDate = request.JoiningDate,
                        YearsOfExperience = request.YearsOfExperience,
                        Shift = request.Shift,
                        Desk = request.Ward
                    };
                    _context.ReceptionistProfiles.Add(receptionistProfile);
                    break;

                case "Admin":
                case "Manager":
                    var adminProfile = new AdminProfile
                    {
                        UserId = userId,
                        Department = request.Department,
                        Position = request.Position ?? request.Role,
                        JoiningDate = request.JoiningDate,
                        Permissions = request.Permissions ?? new List<string>()
                    };
                    _context.AdminProfiles.Add(adminProfile);
                    break;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
