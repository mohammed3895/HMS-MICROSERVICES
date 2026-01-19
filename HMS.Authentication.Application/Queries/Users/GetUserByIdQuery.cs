using HMS.Authentication.Application.DTOs.Users;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Users
{
    public class GetUserByIdQuery : IRequest<Result<GetUserResponse>>
    {
        public Guid UserId { get; set; }
    }
}
