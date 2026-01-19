using AutoMapper;
using HMS.Authentication.Application.Commands.Users;
using HMS.Authentication.Application.DTOs;
using HMS.Authentication.Application.DTOs.Profile;
using HMS.Authentication.Application.DTOs.Roles;
using HMS.Authentication.Application.DTOs.Users;
using HMS.Authentication.Domain.Entities;

namespace HMS.Authentication.Application.Mappings
{
    public class AuthenticationMappingProfile : Profile
    {
        public AuthenticationMappingProfile()
        {
            // ==================== User Mappings ====================

            CreateMap<ApplicationUser, GetUserResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<ApplicationUser, CreateUserResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<ApplicationUser, UpdateUserResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<UpdateUserCommand, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ==================== Profile Mappings ====================

            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<UpdateUserProfileCommand, UserProfile>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ==================== Role-Specific Profile Mappings ====================

            CreateMap<DoctorProfile, DoctorProfileDto>();
            CreateMap<NurseProfile, NurseProfileDto>();
            CreateMap<PharmacistProfile, PharmacistProfileDto>();
            CreateMap<LabTechnicianProfile, LabTechnicianProfileDto>();
            CreateMap<ReceptionistProfile, ReceptionistProfileDto>();
            CreateMap<AdminProfile, AdminProfileDto>();

            // ==================== Role Mappings ====================

            CreateMap<ApplicationRole, GetRoleResponse>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            // ==================== Audit Mappings ====================

            CreateMap<AuditLog, AuditLogDto>();
        }
    }
}