using HMS.Appointment.Application.DTOs;
using HMS.Appointment.Application.Queries;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class GetPatientAppointmentsQueryHandler
        : IRequestHandler<GetPatientAppointmentsQuery, Result<List<AppointmentSummaryDto>>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<GetPatientAppointmentsQueryHandler> _logger;

        public GetPatientAppointmentsQueryHandler(
            AppointmentDbContext context,
            ILogger<GetPatientAppointmentsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<AppointmentSummaryDto>>> Handle(
            GetPatientAppointmentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Appointments
                    .Where(a => a.PatientId == request.PatientId);

                // Apply date filters
                if (request.FromDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate.Date >= request.FromDate.Value.Date);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate.Date <= request.ToDate.Value.Date);
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    if (Enum.TryParse<AppointmentStatus>(request.Status, out var status))
                    {
                        query = query.Where(a => a.Status == status);
                    }
                }

                var appointments = await query
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .Select(a => new AppointmentSummaryDto
                    {
                        Id = a.Id,
                        AppointmentNumber = a.AppointmentNumber,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        DoctorName = a.DoctorName,
                        Department = a.Department,
                        Status = a.Status.ToString(),
                        RoomNumber = a.RoomNumber
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation(
                    "Retrieved {Count} appointments for patient {PatientId}",
                    appointments.Count, request.PatientId);

                return Result<List<AppointmentSummaryDto>>.Success(
                    appointments,
                    "Patient appointments retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient appointments");
                return Result<List<AppointmentSummaryDto>>.Failure(
                    "An error occurred while retrieving appointments");
            }
        }
    }
}