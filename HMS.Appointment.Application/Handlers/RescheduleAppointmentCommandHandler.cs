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
    public class RescheduleAppointmentCommandHandler
         : IRequestHandler<RescheduleAppointmentCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RescheduleAppointmentCommandHandler> _logger;

        public RescheduleAppointmentCommandHandler(
            AppointmentDbContext context,
            IHttpClientFactory httpClientFactory,
            ILogger<RescheduleAppointmentCommandHandler> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            RescheduleAppointmentCommand request,
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

                if (appointment.Status == AppointmentStatus.Cancelled ||
                    appointment.Status == AppointmentStatus.Completed)
                {
                    return Result<bool>.Failure($"Cannot reschedule a {appointment.Status.ToString().ToLower()} appointment");
                }

                // Check new time slot availability
                var newEndTime = request.NewStartTime.Add(TimeSpan.FromMinutes(appointment.DurationMinutes));

                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.Id != request.AppointmentId
                        && a.DoctorId == appointment.DoctorId
                        && a.AppointmentDate.Date == request.NewAppointmentDate.Date
                        && a.Status != AppointmentStatus.Cancelled
                        && a.Status != AppointmentStatus.NoShow
                        && ((a.StartTime < newEndTime && a.EndTime > request.NewStartTime)))
                    .FirstOrDefaultAsync(cancellationToken);

                if (conflictingAppointment != null)
                {
                    return Result<bool>.Failure("New time slot is not available");
                }

                // Check doctor schedule
                var dayOfWeek = request.NewAppointmentDate.DayOfWeek;
                var schedule = await _context.DoctorSchedules
                    .Where(s => s.DoctorId == appointment.DoctorId
                        && s.DayOfWeek == dayOfWeek
                        && s.IsActive
                        && s.StartTime <= request.NewStartTime
                        && s.EndTime >= newEndTime)
                    .FirstOrDefaultAsync(cancellationToken);

                if (schedule == null)
                {
                    return Result<bool>.Failure("New appointment time is outside doctor's working hours");
                }

                // Store old values for history
                var oldDate = appointment.AppointmentDate;
                var oldStartTime = appointment.StartTime;

                // Update appointment
                appointment.AppointmentDate = request.NewAppointmentDate;
                appointment.StartTime = request.NewStartTime;
                appointment.EndTime = newEndTime;
                appointment.UpdatedAt = DateTime.UtcNow;

                // Add history entry
                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "Rescheduled",
                    OldValue = $"{oldDate:yyyy-MM-dd} at {oldStartTime}",
                    NewValue = $"{request.NewAppointmentDate:yyyy-MM-dd} at {request.NewStartTime}",
                    Reason = request.Reason,
                    PerformedBy = request.RescheduledBy,
                    PerformedByName = "System",
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);

                // Update reminders
                var reminders = await _context.AppointmentReminders
                    .Where(r => r.AppointmentId == request.AppointmentId && !r.IsSent)
                    .ToListAsync(cancellationToken);

                foreach (var reminder in reminders)
                {
                    if (reminder.Timing == ReminderTiming.TwentyFourHours)
                    {
                        reminder.ScheduledFor = request.NewAppointmentDate.Add(request.NewStartTime).AddHours(-24);
                    }
                    else if (reminder.Timing == ReminderTiming.TwoHours)
                    {
                        reminder.ScheduledFor = request.NewAppointmentDate.Add(request.NewStartTime).AddHours(-2);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Send notification
                if (request.NotifyPatient)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var notificationClient = _httpClientFactory.CreateClient("NotificationService");
                            await notificationClient.PostAsJsonAsync("/api/notifications/appointment-rescheduled",
                                new
                                {
                                    AppointmentId = appointment.Id,
                                    PatientEmail = appointment.PatientEmail,
                                    PatientPhone = appointment.PatientPhone,
                                    OldDate = oldDate,
                                    OldStartTime = oldStartTime,
                                    NewDate = request.NewAppointmentDate,
                                    NewStartTime = request.NewStartTime,
                                    Reason = request.Reason
                                });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send reschedule notification");
                        }
                    }, cancellationToken);
                }

                _logger.LogInformation(
                    "Appointment {AppointmentNumber} rescheduled from {OldDate} to {NewDate}",
                    appointment.AppointmentNumber, oldDate, request.NewAppointmentDate);

                return Result<bool>.Success(true, "Appointment rescheduled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rescheduling appointment");
                return Result<bool>.Failure("An error occurred while rescheduling the appointment");
            }
        }
    }

}
