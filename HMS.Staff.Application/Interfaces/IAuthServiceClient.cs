using HMS.Staff.Application.DTOs;

namespace HMS.Staff.Application.Interfaces
{
    public interface IAuthServiceClient
    {
        Task<CreateStaffAuthResponse> CreateStaffUserAsync(CreateStaffAuthRequest request);
        Task<UserInfoResponse?> GetUserInfoAsync(Guid userId);
        Task<List<UserInfoResponse>> GetUsersInfoAsync(List<Guid> userIds);
        Task<bool> UpdateUserBasicInfoAsync(Guid userId, UpdateUserBasicInfoRequest request);
    }
}
