using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdateDoctorProfileCommand : IRequest<Result<DoctorProfileDto>>
    {
        public Guid UserId { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? Qualification { get; set; }
        public List<string>? Certifications { get; set; }
        public string? ConsultationFee { get; set; }
        public int? MaxPatientsPerDay { get; set; }
        public string? Biography { get; set; }
        public bool? IsAvailableForConsultation { get; set; }
    }
}
