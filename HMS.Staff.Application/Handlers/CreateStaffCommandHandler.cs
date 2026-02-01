using HMS.Common.DTOs;
using HMS.Staff.Application.Commands;
using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Interfaces;
using HMS.Staff.Domain.Enums;
using HMS.Staff.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Application.Handlers
{
    public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Result<StaffWithUserInfoDto>>
    {
        private readonly StaffDbContext _context;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<CreateStaffCommandHandler> _logger;

        public CreateStaffCommandHandler(
            StaffDbContext context,
            IAuthServiceClient authServiceClient,
            ILogger<CreateStaffCommandHandler> logger)
        {
            _context = context;
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        public async Task<Result<StaffWithUserInfoDto>> Handle(
            CreateStaffCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                // 1. Validate email doesn't exist
                var existingStaff = await _context.Staff
                    .AnyAsync(s => s.UserId != Guid.Empty, cancellationToken);
                // Note: We'll check email via Auth service

                // 2. Generate Staff Number
                var staffCount = await _context.Staff.CountAsync(cancellationToken);
                var staffNumber = $"STF-{DateTime.UtcNow.Year}-{(staffCount + 1):D6}";

                // 3. Create Staff record first (without UserId)
                var staff = new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = staffNumber,
                    UserId = Guid.Empty, // Will be updated after Auth service call
                    StaffType = Enum.Parse<StaffType>(data.StaffType),
                    Department = data.Department,
                    Specialization = data.Specialization,
                    Position = data.Position,
                    JoinDate = data.JoinDate,
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = Enum.Parse<EmploymentType>(data.EmploymentType),
                    ShiftType = Enum.Parse<ShiftType>(data.ShiftType),
                    LicenseNumber = data.LicenseNumber,
                    LicenseIssueDate = data.LicenseIssueDate,
                    LicenseExpiryDate = data.LicenseExpiryDate,
                    Qualifications = data.Qualifications,
                    YearsOfExperience = data.YearsOfExperience,
                    BasicSalary = data.BasicSalary,
                    BankAccountNumber = data.BankAccountNumber,
                    BankName = data.BankName,
                    AddressLine1 = data.AddressLine1,
                    AddressLine2 = data.AddressLine2,
                    City = data.City,
                    State = data.State,
                    Country = data.Country,
                    PostalCode = data.PostalCode,
                    EmergencyContactName = data.EmergencyContactName,
                    EmergencyContactPhone = data.EmergencyContactPhone,
                    EmergencyContactRelationship = data.EmergencyContactRelationship,
                    IsActive = true,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                // Save temporarily
                _context.Staff.Add(staff);
                await _context.SaveChangesAsync(cancellationToken);

                // 4. Call Auth Service to create user account
                var authRequest = new CreateStaffAuthRequest
                {
                    Email = data.Email,
                    Password = data.Password,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    DateOfBirth = data.DateOfBirth,
                    PhoneNumber = data.PhoneNumber,
                    NationalId = data.NationalId,
                    Role = data.StaffType,
                    StaffServiceId = staff.Id
                };

                var authResponse = await _authServiceClient.CreateStaffUserAsync(authRequest);

                if (!authResponse.Success)
                {
                    // Rollback: Delete staff record
                    _context.Staff.Remove(staff);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogError("Failed to create user in Auth service: {Errors}",
                        string.Join(", ", authResponse.Errors));

                    return Result<StaffWithUserInfoDto>.Failure(
                        $"Failed to create user account: {authResponse.Message}");
                }

                // 5. Update Staff record with UserId
                staff.UserId = authResponse.UserId;
                _context.Staff.Update(staff);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Staff created successfully: {StaffNumber} - UserId: {UserId}",
                    staff.StaffNumber, staff.UserId);

                // 6. Return complete staff info
                var result = new StaffWithUserInfoDto
                {
                    Id = staff.Id,
                    StaffNumber = staff.StaffNumber,
                    UserId = staff.UserId,
                    Email = data.Email,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    PhoneNumber = data.PhoneNumber,
                    DateOfBirth = data.DateOfBirth,
                    NationalId = data.NationalId,
                    StaffType = staff.StaffType.ToString(),
                    Department = staff.Department,
                    Specialization = staff.Specialization,
                    Position = staff.Position,
                    BasicSalary = staff.BasicSalary,
                    EmploymentStatus = staff.EmploymentStatus.ToString()
                };

                return Result<StaffWithUserInfoDto>.Success(
                    result,
                    "Staff member created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff with authentication");
                return Result<StaffWithUserInfoDto>.Failure(
                    "An error occurred while creating staff member");
            }
        }
    }
}
