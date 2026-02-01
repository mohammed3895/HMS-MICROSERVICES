using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class StartConsultationCommandHandler
          : IRequestHandler<StartConsultationCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<StartConsultationCommandHandler> _logger;

        public StartConsultationCommandHandler(
            AppointmentDbContext context,
            ILogger<StartConsultationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            StartConsultationCommand request,
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

                if (appointment.DoctorId != request.DoctorId)
                {
                    return Result<bool>.Failure("Unauthorized: This appointment belongs to a different doctor");
                }

                if (appointment.Status != AppointmentStatus.CheckedIn)
                {
                    return Result<bool>.Failure("Patient must be checked in before starting consultation");
                }

                if (appointment.Status == AppointmentStatus.InProgress)
                {
                    return Result<bool>.Failure("Consultation has already started");
                }

                appointment.Status = AppointmentStatus.InProgress;
                appointment.ConsultationStartTime = DateTime.UtcNow;
                appointment.RoomNumber = request.RoomNumber;
                appointment.UpdatedAt = DateTime.UtcNow;

                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "ConsultationStarted",
                    NewValue = $"Consultation started in room {request.RoomNumber}",
                    PerformedBy = request.DoctorId,
                    PerformedByName = appointment.DoctorName,
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Consultation started for appointment {AppointmentNumber} by doctor {DoctorId}",
                    appointment.AppointmentNumber, request.DoctorId);

                return Result<bool>.Success(true, "Consultation started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting consultation");
                return Result<bool>.Failure("An error occurred while starting consultation");
            }
        }
    }
}
