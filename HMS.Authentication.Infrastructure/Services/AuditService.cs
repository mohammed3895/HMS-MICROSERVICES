using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly AuthenticationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(
            AuthenticationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            string action,
            string entityName,
            string? entityId = null,
            string? oldValues = null,
            string? newValues = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = GetCurrentUserId();

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLog>> GetUserAuditLogsAsync(
            Guid userId,
            int pageNumber = 1,
            int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetEntityAuditLogsAsync(
            string entityName,
            string entityId,
            int pageNumber = 1,
            int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetAuditLogsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int pageNumber = 1,
            int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}