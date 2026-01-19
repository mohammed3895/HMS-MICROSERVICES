using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class GenerateRecoveryCodesCommand : IRequest<Result<GenerateRecoveryCodesResponse>>
    {
        public Guid UserId { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
