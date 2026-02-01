using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HMS.Appointment.Application.Handlers
{
    public class CancelAppointmentCommandHandler
         : IRequestHandler<CancelAppointmentCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CancelAppointmentCommandHandler> _logger;

        public CancelAppointmentCommandHandler(
            AppointmentDbContext context,
            IHttpClientFactory httpClientFactory,
            ILogger<CancelAppointmentCommandHandler> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            CancelAppointmentCommand request,
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

                if (appointment.Status == AppointmentStatus.Cancelled)
                {
                    return Result<bool>.Failure("Appointment is already cancelled");
                }

                if (appointment.Status == AppointmentStatus.Completed)
                {
                    return Result<bool>.Failure("Cannot cancel a completed appointment");
                }

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.CancellationReason = request.CancellationReason;
                appointment.CancelledBy = request.CancelledBy;
                appointment.CancelledAt = DateTime.UtcNow;
                appointment.UpdatedAt = DateTime.UtcNow;

                // Add history entry
                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "Cancelled",
                    OldValue = appointment.Status.ToString(),
                    NewValue = AppointmentStatus.Cancelled.ToString(),
                    Reason = request.CancellationReason,
                    PerformedBy = request.CancelledBy,
                    PerformedByName = "System",
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);
                await _context.SaveChangesAsync(cancellationToken);

                // Send notification
                if (request.NotifyPatient)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var notificationClient = _httpClientFactory.CreateClient("NotificationService");
                            await notificationClient.PostAsJsonAsync("/api/notifications/appointment-cancellation",
                                new
                                {
                                    AppointmentId = appointment.Id,
                                    PatientEmail = appointment.PatientEmail,
                                    PatientPhone = appointment.PatientPhone,
                                    CancellationReason = request.CancellationReason,
                                    AllowRefund = request.AllowRefund
                                });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send cancellation notification");
                        }
                    }, cancellationToken);
                }

                _logger.LogInformation(
                    "Appointment {AppointmentNumber} cancelled by {CancelledBy}",
                    appointment.AppointmentNumber, request.CancelledBy);

                return Result<bool>.Success(true, "Appointment cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment");
                return Result<bool>.Failure("An error occurred while cancelling the appointment");
            }
        }
    }
}
