using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Infrastructure.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly AuthenticationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public DeviceService(
            AuthenticationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<UserDevice> RegisterOrUpdateDeviceAsync(Guid userId, string deviceId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();

            var existingDevice = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);

            if (existingDevice != null)
            {
                existingDevice.LastUsedAt = DateTime.UtcNow;
                existingDevice.IpAddress = ipAddress;
                existingDevice.UserAgent = userAgent;
                _context.UserDevices.Update(existingDevice);
            }
            else
            {
                var deviceInfo = ParseUserAgent(userAgent);
                var newDevice = new UserDevice
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DeviceId = deviceId,
                    DeviceName = deviceInfo.DeviceName,
                    DeviceType = deviceInfo.DeviceType,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    IsTrusted = false,
                    LastUsedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserDevices.Add(newDevice);

                // Send email notification for new device
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    await _emailService.SendNewDeviceLoginEmailAsync(user, deviceInfo.DeviceName);
                }

                existingDevice = newDevice;
            }

            await _context.SaveChangesAsync();
            return existingDevice;
        }

        public async Task<bool> IsDeviceTrustedAsync(Guid userId, string deviceId)
        {
            var device = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);

            return device?.IsTrusted ?? false;
        }

        public async Task TrustDeviceAsync(Guid userId, string deviceId)
        {
            var device = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);

            if (device != null)
            {
                device.IsTrusted = true;
                _context.UserDevices.Update(device);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserDevice>> GetUserDevicesAsync(Guid userId)
        {
            return await _context.UserDevices
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.LastUsedAt)
                .ToListAsync();
        }

        public async Task RemoveDeviceAsync(Guid userId, Guid deviceId)
        {
            var device = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.Id == deviceId);

            if (device != null)
            {
                _context.UserDevices.Remove(device);
                await _context.SaveChangesAsync();
            }
        }

        private (string DeviceName, string DeviceType) ParseUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return ("Unknown Device", "Unknown");

            var ua = userAgent.ToLower();

            // Determine device type
            string deviceType;
            if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
                deviceType = "Mobile";
            else if (ua.Contains("tablet") || ua.Contains("ipad"))
                deviceType = "Tablet";
            else
                deviceType = "Desktop";

            // Determine device name
            string deviceName;
            if (ua.Contains("windows"))
                deviceName = "Windows PC";
            else if (ua.Contains("mac"))
                deviceName = "Mac";
            else if (ua.Contains("linux"))
                deviceName = "Linux PC";
            else if (ua.Contains("iphone"))
                deviceName = "iPhone";
            else if (ua.Contains("ipad"))
                deviceName = "iPad";
            else if (ua.Contains("android"))
                deviceName = "Android Device";
            else
                deviceName = "Unknown Device";

            // Add browser info
            if (ua.Contains("chrome"))
                deviceName += " (Chrome)";
            else if (ua.Contains("firefox"))
                deviceName += " (Firefox)";
            else if (ua.Contains("safari") && !ua.Contains("chrome"))
                deviceName += " (Safari)";
            else if (ua.Contains("edge"))
                deviceName += " (Edge)";

            return (deviceName, deviceType);
        }
    }
}