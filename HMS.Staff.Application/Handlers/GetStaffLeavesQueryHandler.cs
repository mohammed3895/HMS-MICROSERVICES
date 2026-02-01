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
    public class GetStaffLeavesQueryHandler : IRequestHandler<GetStaffLeavesQuery, Result<List<StaffLeaveDto>>>
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<GetStaffLeavesQueryHandler> _logger;

        public GetStaffLeavesQueryHandler(StaffDbContext context, ILogger<GetStaffLeavesQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<StaffLeaveDto>>> Handle(
            GetStaffLeavesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.StaffLeaves
                    .Include(l => l.Staff)
                    .Where(l => l.StaffId == request.StaffId);

                if (request.FromDate.HasValue)
                {
                    query = query.Where(l => l.StartDate >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(l => l.EndDate <= request.ToDate.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    if (Enum.TryParse<LeaveStatus>(request.Status, out var status))
                    {
                        query = query.Where(l => l.Status == status);
                    }
                }

                var leaves = await query
                    .OrderByDescending(l => l.CreatedAt)
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
                _logger.LogError(ex, "Error retrieving staff leaves for {StaffId}", request.StaffId);
                return Result<List<StaffLeaveDto>>.Failure("Error retrieving staff leaves");
            }
        }
    }
}
