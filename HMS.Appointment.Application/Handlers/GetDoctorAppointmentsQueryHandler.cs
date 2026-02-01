using HMS.Appointment.Application.DTOs;
using HMS.Appointment.Application.Queries;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class GetDoctorAppointmentsQueryHandler
        : IRequestHandler<GetDoctorAppointmentsQuery, Result<List<AppointmentDetailsDto>>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<GetDoctorAppointmentsQueryHandler> _logger;

        public GetDoctorAppointmentsQueryHandler(
            AppointmentDbContext context,
            ILogger<GetDoctorAppointmentsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<AppointmentDetailsDto>>> Handle(
            GetDoctorAppointmentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Where(a => a.DoctorId == request.DoctorId
                        && a.AppointmentDate.Date == request.Date.Date)
                    .OrderBy(a => a.StartTime)
                    .Select(a => new AppointmentDetailsDto
                    {
                        Id = a.Id,
                        AppointmentNumber = a.AppointmentNumber,
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        PatientName = a.PatientName,
                        PatientPhone = a.PatientPhone,
                        Status = a.Status.ToString(),
                        ChiefComplaint = a.ChiefComplaint,
                        RoomNumber = a.RoomNumber,
                        IsCheckedIn = a.CheckInTime.HasValue
                    })
                    .ToListAsync(cancellationToken);

                _logger.LogInformation(
                    "Retrieved {Count} appointments for doctor {DoctorId} on {Date}",
                    appointments.Count, request.DoctorId, request.Date.Date);

                return Result<List<AppointmentDetailsDto>>.Success(
                    appointments,
                    "Doctor appointments retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor appointments");
                return Result<List<AppointmentDetailsDto>>.Failure(
                    "An error occurred while retrieving appointments");
            }
        }
    }
}