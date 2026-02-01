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
    public class SendAppointmentReminderCommandHandler
          : IRequestHandler<SendAppointmentReminderCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SendAppointmentReminderCommandHandler> _logger;

        public SendAppointmentReminderCommandHandler(
            AppointmentDbContext context,
            IHttpClientFactory httpClientFactory,
            ILogger<SendAppointmentReminderCommandHandler> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            SendAppointmentReminderCommand request,
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

                var reminderType = Enum.Parse<ReminderType>(request.ReminderType);
                var reminderTiming = Enum.Parse<ReminderTiming>(request.Timing);

                // Create or update reminder
                var reminder = await _context.AppointmentReminders
                    .FirstOrDefaultAsync(r => r.AppointmentId == request.AppointmentId
                        && r.Type == reminderType
                        && r.Timing == reminderTiming,
                        cancellationToken);

                if (reminder == null)
                {
                    reminder = new Domain.Entities.AppointmentReminder
                    {
                        Id = Guid.NewGuid(),
                        AppointmentId = request.AppointmentId,
                        Type = reminderType,
                        Timing = reminderTiming,
                        ScheduledFor = DateTime.UtcNow,
                        IsSent = false
                    };
                    _context.AppointmentReminders.Add(reminder);
                }

                // Send reminder
                var notificationClient = _httpClientFactory.CreateClient("NotificationService");
                await notificationClient.PostAsJsonAsync("/api/notifications/appointment-reminder",
                    new
                    {
                        AppointmentId = appointment.Id,
                        ReminderType = request.ReminderType,
                        PatientEmail = appointment.PatientEmail,
                        PatientPhone = appointment.PatientPhone,
                        AppointmentDetails = new
                        {
                            appointment.AppointmentNumber,
                            appointment.AppointmentDate,
                            appointment.StartTime,
                            appointment.DoctorName,
                            appointment.Department
                        }
                    }, cancellationToken);

                reminder.IsSent = true;
                reminder.SentAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Reminder sent for appointment {AppointmentNumber} via {Type}",
                    appointment.AppointmentNumber, request.ReminderType);

                return Result<bool>.Success(true, "Reminder sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending appointment reminder");
                return Result<bool>.Failure("An error occurred while sending reminder");
            }
        }
    }
}
