using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Interfaces;

namespace HMS.Staff.Application.Helpers
{
    public static class StaffMappingExtensions
    {
        public static async Task<List<StaffSummaryDto>> MapToStaffSummaryDtos(
            this IEnumerable<Domain.Entities.Staff> staffList,
            IAuthServiceClient authServiceClient)
        {
            var staffArray = staffList.ToList();
            var userIds = staffArray.Select(s => s.UserId).Where(id => id != Guid.Empty).ToList();
            var usersInfo = await authServiceClient.GetUsersInfoAsync(userIds);
            var userDict = usersInfo.ToDictionary(u => u.UserId, u => u);

            return staffArray.Select(s =>
            {
                userDict.TryGetValue(s.UserId, out var userInfo);
                return new StaffSummaryDto
                {
                    Id = s.Id,
                    StaffNumber = s.StaffNumber,
                    FullName = userInfo != null ? $"{userInfo.FirstName} {userInfo.LastName}" : "N/A",
                    Email = userInfo?.Email ?? "",
                    PhoneNumber = userInfo?.PhoneNumber ?? "",
                    ProfilePictureUrl = userInfo?.ProfilePictureUrl ?? "",
                    StaffType = s.StaffType.ToString(),
                    Department = s.Department,
                    Position = s.Position,
                    EmploymentStatus = s.EmploymentStatus.ToString(),
                    YearsOfExperience = s.YearsOfExperience
                };
            }).ToList();
        }
    }
}
