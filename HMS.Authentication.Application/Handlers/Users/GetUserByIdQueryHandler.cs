using AutoMapper;
using HMS.Authentication.Application.DTOs.Users;
using HMS.Authentication.Application.Queries.Users;
using HMS.Authentication.Domain.Entities;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMS.Authentication.Application.Handlers.Users
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result<GetUserResponse>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
                return Result<GetUserResponse>.Failure("User not found");

            var response = _mapper.Map<GetUserResponse>(user);
            var roles = await _userManager.GetRolesAsync(user);
            response.Roles = roles.ToList();

            return Result<GetUserResponse>.Success(response);
        }
    }
}
