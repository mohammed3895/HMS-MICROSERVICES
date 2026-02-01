using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Interfaces;
using HMS.Staff.Application.Queries;
using HMS.Staff.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Application.Handlers
{
    public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, Result<StaffDetailsDto>>
    {
        private readonly StaffDbContext _context;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<GetStaffByIdQueryHandler> _logger;

        public GetStaffByIdQueryHandler(
            StaffDbContext context,
            IAuthServiceClient authServiceClient,
            ILogger<GetStaffByIdQueryHandler> logger)
        {
            _context = context;
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        public async Task<Result<StaffDetailsDto>> Handle(
            GetStaffByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var staff = await _context.Staff
                    .Include(s => s.EducationRecords)
                    .Include(s => s.Certifications)
                    .FirstOrDefaultAsync(s => s.Id == request.StaffId, cancellationToken);

                if (staff == null)
                {
                    return Result<StaffDetailsDto>.Failure("Staff member not found");
                }

                // Fetch user info from Auth service
                var userInfo = await _authServiceClient.GetUserInfoAsync(staff.UserId);

                var dto = new StaffDetailsDto
                {
                    Id = staff.Id,
                    StaffNumber = staff.StaffNumber,
                    FullName = userInfo != null ? $"{userInfo.FirstName} {userInfo.LastName}" : "N/A",
                    FirstName = userInfo?.FirstName ?? "",
                    LastName = userInfo?.LastName ?? "",
                    Email = userInfo?.Email ?? "",
                    PhoneNumber = userInfo?.PhoneNumber ?? "",
                    AlternatePhoneNumber = "",
                    DateOfBirth = userInfo?.DateOfBirth ?? DateTime.MinValue,
                    Gender = "",
                    ProfilePictureUrl = userInfo?.ProfilePictureUrl ?? "",
                    NationalId = "",
                    StaffType = staff.StaffType.ToString(),
                    Department = staff.Department,
                    Specialization = staff.Specialization,
                    Position = staff.Position,
                    JoinDate = staff.JoinDate,
                    EndDate = staff.EndDate,
                    EmploymentStatus = staff.EmploymentStatus.ToString(),
                    EmploymentType = staff.EmploymentType.ToString(),
                    ShiftType = staff.ShiftType.ToString(),
                    LicenseNumber = staff.LicenseNumber,
                    LicenseExpiryDate = staff.LicenseExpiryDate,
                    Qualifications = staff.Qualifications,
                    YearsOfExperience = staff.YearsOfExperience,
                    BasicSalary = staff.BasicSalary,
                    AddressLine1 = staff.AddressLine1,
                    AddressLine2 = staff.AddressLine2,
                    City = staff.City,
                    State = staff.State,
                    Country = staff.Country,
                    PostalCode = staff.PostalCode,
                    EducationRecords = staff.EducationRecords.Select(e => new StaffEducationDto
                    {
                        Id = e.Id,
                        Degree = e.Degree,
                        Institution = e.Institution,
                        FieldOfStudy = e.FieldOfStudy,
                        StartYear = e.StartYear,
                        EndYear = e.EndYear,
                        Country = e.Country
                    }).ToList(),
                    Certifications = staff.Certifications.Select(c => new StaffCertificationDto
                    {
                        Id = c.Id,
                        CertificationName = c.CertificationName,
                        IssuingOrganization = c.IssuingOrganization,
                        IssueDate = c.IssueDate,
                        ExpiryDate = c.ExpiryDate
                    }).ToList()
                };

                return Result<StaffDetailsDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff details for {StaffId}", request.StaffId);
                return Result<StaffDetailsDto>.Failure("Error retrieving staff details");
            }
        }
    }
}
