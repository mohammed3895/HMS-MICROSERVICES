using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class CheckInAppointmentCommandHandler
         : IRequestHandler<CheckInAppointmentCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<CheckInAppointmentCommandHandler> _logger;

        public CheckInAppointmentCommandHandler(
            AppointmentDbContext context,
            ILogger<CheckInAppointmentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            CheckInAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

                if (appointment == null)
                {
                    return Result<bool>.Failure("Appointment not found");
                }

                if (appointment.Status != AppointmentStatus.Scheduled &&
                    appointment.Status != AppointmentStatus.Confirmed)
                {
                    return Result<bool>.Failure("Only scheduled or confirmed appointments can be checked in");
                }

                if (appointment.Status == AppointmentStatus.CheckedIn)
                {
                    return Result<bool>.Failure("Patient is already checked in");
                }

                appointment.CheckInTime = DateTime.UtcNow;
                appointment.CheckInMethod = request.CheckInMethod;
                appointment.UpdatedAt = DateTime.UtcNow;

                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "CheckedIn",
                    NewValue = $"Checked in via {request.CheckInMethod}",
                    Reason = request.AdditionalNotes,
                    PerformedBy = Guid.Empty,
                    PerformedByName = "System",
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Patient checked in for appointment {AppointmentNumber} via {Method}",
                    appointment.AppointmentNumber, request.CheckInMethod);

                return Result<bool>.Success(true, "Patient checked in successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking in appointment");
                return Result<bool>.Failure("An error occurred while checking in");
            }
        }
    }

}
