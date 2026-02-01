using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class MarkNoShowCommandHandler
        : IRequestHandler<MarkNoShowCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<MarkNoShowCommandHandler> _logger;

        public MarkNoShowCommandHandler(
            AppointmentDbContext context,
            ILogger<MarkNoShowCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            MarkNoShowCommand request,
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

                if (appointment.Status == AppointmentStatus.Completed ||
                    appointment.Status == AppointmentStatus.Cancelled)
                {
                    return Result<bool>.Failure($"Cannot mark {appointment.Status.ToString().ToLower()} appointment as no-show");
                }

                appointment.Status = AppointmentStatus.NoShow;
                appointment.UpdatedBy = request.MarkedBy;
                appointment.UpdatedAt = DateTime.UtcNow;

                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "MarkedNoShow",
                    NewValue = AppointmentStatus.NoShow.ToString(),
                    Reason = request.Notes,
                    PerformedBy = request.MarkedBy,
                    PerformedByName = "System",
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Appointment {AppointmentNumber} marked as no-show",
                    appointment.AppointmentNumber);

                return Result<bool>.Success(true, "Appointment marked as no-show");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking appointment as no-show");
                return Result<bool>.Failure("An error occurred while marking as no-show");
            }
        }
    }
}
