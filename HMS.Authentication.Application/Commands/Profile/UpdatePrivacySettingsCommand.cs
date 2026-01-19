using HMS.Authentication.Domain.DTOs.Settings;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdatePrivacySettingsCommand : IRequest<Result<PrivacySettingsDto>>
    {
        public Guid UserId { get; set; }
        public bool? ProfileVisibility { get; set; }
        public bool? ShowEmail { get; set; }
        public bool? ShowPhoneNumber { get; set; }
        public bool? AllowDataSharing { get; set; }
    }
}
