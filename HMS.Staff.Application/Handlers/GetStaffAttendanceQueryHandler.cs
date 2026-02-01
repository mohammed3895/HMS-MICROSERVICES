using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Queries;
using HMS.Staff.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Application.Handlers
{
    public class GetStaffAttendanceQueryHandler : IRequestHandler<GetStaffAttendanceQuery, Result<List<StaffAttendanceDto>>>
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<GetStaffAttendanceQueryHandler> _logger;

        public GetStaffAttendanceQueryHandler(StaffDbContext context, ILogger<GetStaffAttendanceQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<StaffAttendanceDto>>> Handle(
            GetStaffAttendanceQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.StaffAttendances
                    .Where(a => a.StaffId == request.StaffId);

                if (request.FromDate.HasValue)
                {
                    query = query.Where(a => a.Date >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(a => a.Date <= request.ToDate.Value);
                }

                var attendance = await query
                    .OrderByDescending(a => a.Date)
                    .Select(a => new StaffAttendanceDto
                    {
                        Id = a.Id,
                        StaffId = a.StaffId,
                        Date = a.Date,
                        CheckInTime = a.CheckInTime,
                        CheckOutTime = a.CheckOutTime,
                        Status = a.Status.ToString(),
                        Notes = a.Notes
                    })
                    .ToListAsync(cancellationToken);

                return Result<List<StaffAttendanceDto>>.Success(attendance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving attendance for {StaffId}", request.StaffId);
                return Result<List<StaffAttendanceDto>>.Failure("Error retrieving attendance");
            }
        }
    }

}
