using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class CompleteAppointmentCommandHandler
         : IRequestHandler<CompleteAppointmentCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<CompleteAppointmentCommandHandler> _logger;

        public CompleteAppointmentCommandHandler(
            AppointmentDbContext context,
            ILogger<CompleteAppointmentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            CompleteAppointmentCommand request,
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

                if (appointment.Status != AppointmentStatus.InProgress)
                {
                    return Result<bool>.Failure("Only in-progress appointments can be completed");
                }

                appointment.Status = AppointmentStatus.Completed;
                appointment.ConsultationEndTime = DateTime.UtcNow;
                appointment.Notes = request.ConsultationNotes;
                appointment.RequiresFollowUp = request.RequiresFollowUp;
                appointment.FollowUpDate = request.FollowUpDate;
                appointment.UpdatedAt = DateTime.UtcNow;

                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "Completed",
                    NewValue = "Consultation completed",
                    //Reason = request.RequiresFollowUp ? $"Follow-up required on {request.FollowUpDate:yyyy-MM-dd}" : null,
                    PerformedBy = request.CompletedBy,
                    PerformedByName = "Doctor",
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Appointment {AppointmentNumber} completed by {CompletedBy}",
                    appointment.AppointmentNumber, request.CompletedBy);

                return Result<bool>.Success(true, "Appointment completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing appointment");
                return Result<bool>.Failure("An error occurred while completing the appointment");
            }
        }
    }
}
