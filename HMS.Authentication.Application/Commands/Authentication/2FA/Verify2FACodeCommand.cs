using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class Verify2FACodeCommand : IRequest<Result<LoginResponse>>
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string TwoFactorToken { get; set; } = string.Empty;

        public bool TrustDevice { get; set; } = false;

        public string? DeviceId { get; set; }
    }
}
