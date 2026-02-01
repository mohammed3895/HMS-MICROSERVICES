using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class BulkRescheduleAppointmentsCommandHandler
        : IRequestHandler<BulkRescheduleAppointmentsCommand, Result<int>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<BulkRescheduleAppointmentsCommandHandler> _logger;

        public BulkRescheduleAppointmentsCommandHandler(
            AppointmentDbContext context,
            ILogger<BulkRescheduleAppointmentsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(
            BulkRescheduleAppointmentsCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Where(a => a.DoctorId == request.DoctorId
                        && a.AppointmentDate.Date >= request.FromDate.Date
                        && a.AppointmentDate.Date <= request.ToDate.Date
                        && (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed))
                    .ToListAsync(cancellationToken);

                int count = 0;
                foreach (var appointment in appointments)
                {
                    appointment.Status = AppointmentStatus.Rescheduled;
                    appointment.Notes = $"Bulk reschedule: {request.Reason}";
                    appointment.UpdatedAt = DateTime.UtcNow;

                    var history = new Domain.Entities.AppointmentHistory
                    {
                        Id = Guid.NewGuid(),
                        AppointmentId = appointment.Id,
                        Action = "RequiresRescheduling",
                        Reason = request.Reason,
                        PerformedBy = request.RescheduledBy,
                        PerformedByName = "System",
                        PerformedAt = DateTime.UtcNow
                    };

                    _context.AppointmentHistories.Add(history);
                    count++;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Bulk rescheduled {Count} appointments for doctor {DoctorId}",
                    count, request.DoctorId);

                return Result<int>.Success(count, $"{count} appointments marked for rescheduling");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk rescheduling appointments");
                return Result<int>.Failure("An error occurred during bulk reschedule");
            }
        }
    }
}
