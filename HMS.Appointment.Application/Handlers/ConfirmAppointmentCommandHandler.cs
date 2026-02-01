using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class ConfirmAppointmentCommandHandler
          : IRequestHandler<ConfirmAppointmentCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<ConfirmAppointmentCommandHandler> _logger;

        public ConfirmAppointmentCommandHandler(
            AppointmentDbContext context,
            ILogger<ConfirmAppointmentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            ConfirmAppointmentCommand request,
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

                if (appointment.Status != AppointmentStatus.Scheduled)
                {
                    return Result<bool>.Failure("Only scheduled appointments can be confirmed");
                }

                appointment.Status = AppointmentStatus.Confirmed;
                appointment.CreatedAt = DateTime.UtcNow;
                appointment.CheckInMethod = request.ConfirmationMethod;
                appointment.UpdatedAt = DateTime.UtcNow;

                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "Confirmed",
                    NewValue = $"Confirmed via {request.ConfirmationMethod}",
                    PerformedBy = Guid.Empty,
                    PerformedByName = "System",
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Appointment {AppointmentNumber} confirmed via {Method}",
                    appointment.AppointmentNumber, request.ConfirmationMethod);

                return Result<bool>.Success(true, "Appointment confirmed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming appointment");
                return Result<bool>.Failure("An error occurred while confirming the appointment");
            }
        }
    }
}
