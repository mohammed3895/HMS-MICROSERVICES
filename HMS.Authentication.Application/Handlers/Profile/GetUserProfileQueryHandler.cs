using AutoMapper;
using HMS.Authentication.Application.DTOs.Profile;
using HMS.Authentication.Application.Queries.Profile;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationDbContext _context;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(
            UserManager<ApplicationUser> userManager,
            AuthenticationDbContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<UserProfileResponse>> Handle(
            GetUserProfileQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return Result<UserProfileResponse>.Failure("User not found");

            var response = new UserProfileResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DateOfBirth = user.DateOfBirth,
                NationalId = user.NationalId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);
            response.Roles = roles.ToList();

            // Get general profile
            if (user.Profile != null)
            {
                response.Address = user.Profile.Address;
                response.City = user.Profile.City;
                response.State = user.Profile.State;
                response.ZipCode = user.Profile.ZipCode;
                response.Country = user.Profile.Country;
                response.EmergencyContactName = user.Profile.EmergencyContactName;
                response.EmergencyContactPhone = user.Profile.EmergencyContactPhone;
                response.BloodGroup = user.Profile.BloodGroup;
                response.Allergies = user.Profile.Allergies;
            }

            // Get role-specific profiles
            foreach (var role in roles)
            {
                switch (role)
                {
                    case "Doctor":
                        var doctorProfile = await _context.DoctorProfiles
                            .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
                        if (doctorProfile != null)
                            response.DoctorProfile = _mapper.Map<DoctorProfileDto>(doctorProfile);
                        break;

                    case "Nurse":
                        var nurseProfile = await _context.NurseProfiles
                            .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
                        if (nurseProfile != null)
                            response.NurseProfile = _mapper.Map<NurseProfileDto>(nurseProfile);
                        break;

                    case "Pharmacist":
                        var pharmacistProfile = await _context.PharmacistProfiles
                            .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
                        if (pharmacistProfile != null)
                            response.PharmacistProfile = _mapper.Map<PharmacistProfileDto>(pharmacistProfile);
                        break;

                    case "LabTechnician":
                        var labProfile = await _context.LabTechnicianProfiles
                            .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
                        if (labProfile != null)
                            response.LabTechnicianProfile = _mapper.Map<LabTechnicianProfileDto>(labProfile);
                        break;

                    case "Receptionist":
                        var receptionistProfile = await _context.ReceptionistProfiles
                            .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
                        if (receptionistProfile != null)
                            response.ReceptionistProfile = _mapper.Map<ReceptionistProfileDto>(receptionistProfile);
                        break;

                    case "Admin":
                    case "Manager":
                        var adminProfile = await _context.AdminProfiles
                            .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
                        if (adminProfile != null)
                            response.AdminProfile = _mapper.Map<AdminProfileDto>(adminProfile);
                        break;
                }
            }

            return Result<UserProfileResponse>.Success(response);
        }
    }
}
