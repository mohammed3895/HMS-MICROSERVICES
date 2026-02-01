using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Queries;
using HMS.Staff.Domain.Enums;
using HMS.Staff.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Application.Handlers
{
    public class GetPendingLeaveRequestsQueryHandler : IRequestHandler<GetPendingLeaveRequestsQuery, Result<List<StaffLeaveDto>>>
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<GetPendingLeaveRequestsQueryHandler> _logger;

        public GetPendingLeaveRequestsQueryHandler(StaffDbContext context, ILogger<GetPendingLeaveRequestsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<StaffLeaveDto>>> Handle(
            GetPendingLeaveRequestsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.StaffLeaves
                    .Include(l => l.Staff)
                    .Where(l => l.Status == LeaveStatus.Pending);

                if (request.StaffId.HasValue)
                {
                    query = query.Where(l => l.StaffId == request.StaffId.Value);
                }

                var leaves = await query
                    .OrderBy(l => l.CreatedAt)
                    .Select(l => new StaffLeaveDto
                    {
                        Id = l.Id,
                        StaffId = l.StaffId,
                        StaffName = l.Staff.StaffNumber,
                        LeaveType = l.Type.ToString(),
                        StartDate = l.StartDate,
                        EndDate = l.EndDate,
                        TotalDays = l.TotalDays,
                        Reason = l.Reason,
                        Status = l.Status.ToString(),
                        CreatedAt = l.CreatedAt
                    })
                    .ToListAsync(cancellationToken);

                return Result<List<StaffLeaveDto>>.Success(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending leave requests");
                return Result<List<StaffLeaveDto>>.Failure("Error retrieving pending leave requests");
            }
        }
    }
}
