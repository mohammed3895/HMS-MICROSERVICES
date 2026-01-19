using HMS.Authentication.Application.DTOs.Users;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Users
{
    public class GetAllUsersQuery : IRequest<Result<PaginatedList<GetUserResponse>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}
