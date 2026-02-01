using HMS.Appointment.Application.Commands;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class UpdateDoctorScheduleCommandHandler
         : IRequestHandler<UpdateDoctorScheduleCommand, Result<bool>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<UpdateDoctorScheduleCommandHandler> _logger;

        public UpdateDoctorScheduleCommandHandler(
            AppointmentDbContext context,
            ILogger<UpdateDoctorScheduleCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            UpdateDoctorScheduleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Deactivate existing schedules
                var existingSchedules = await _context.DoctorSchedules
                    .Where(s => s.DoctorId == request.DoctorId && s.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var schedule in existingSchedules)
                {
                    schedule.IsActive = false;
                }

                // Create new schedules
                foreach (var slot in request.Schedule)
                {
                    var newSchedule = new Domain.Entities.DoctorSchedule
                    {
                        Id = Guid.NewGuid(),
                        DoctorId = request.DoctorId,
                        DayOfWeek = slot.DayOfWeek,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        SlotDurationMinutes = slot.SlotDurationMinutes,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.DoctorSchedules.Add(newSchedule);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Doctor schedule updated for doctor {DoctorId}",
                    request.DoctorId);

                return Result<bool>.Success(true, "Doctor schedule updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor schedule");
                return Result<bool>.Failure("An error occurred while updating the schedule");
            }
        }
    }
}
