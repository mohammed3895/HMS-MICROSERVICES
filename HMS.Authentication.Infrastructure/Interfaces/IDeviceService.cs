using HMS.Authentication.Domain.Entities;

namespace HMS.Authentication.Infrastructure.Interfaces
{
    public interface IDeviceService
    {
        Task<UserDevice> RegisterOrUpdateDeviceAsync(Guid userId, string deviceId);
        Task<bool> IsDeviceTrustedAsync(Guid userId, string deviceId);
        Task TrustDeviceAsync(Guid userId, string deviceId);
        Task<List<UserDevice>> GetUserDevicesAsync(Guid userId);
        Task RemoveDeviceAsync(Guid userId, Guid deviceId);
    }
}
