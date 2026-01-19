using HMS.Authentication.Domain.Entities;

namespace HMS.Authentication.Infrastructure.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entityName, string? entityId = null, string? oldValues = null, string? newValues = null);
        Task<List<AuditLog>> GetUserAuditLogsAsync(Guid userId, int pageNumber = 1, int pageSize = 50);
        Task<List<AuditLog>> GetEntityAuditLogsAsync(string entityName, string entityId, int pageNumber = 1, int pageSize = 50);
        Task<List<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50);
    }
}
