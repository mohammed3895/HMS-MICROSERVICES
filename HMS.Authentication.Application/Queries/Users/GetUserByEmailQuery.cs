using HMS.Authentication.Application.DTOs.Users;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Users
{
    public class GetUserByEmailQuery : IRequest<Result<GetUserResponse>>
    {
        public string Email { get; set; }
    }
}
