using HMS.Authentication.Application.DTOs;
using HMS.Authentication.Domain.Enums;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries
{
    public class GetAuditLogsQuery : IRequest<Result<PaginatedList<AuditLogDto>>>
    {
        public Guid? UserId { get; set; }
        public AuditAction? Action { get; set; }
        public string? EntityName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
