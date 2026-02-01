using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class ApplyDoctorLeaveCommandHandler
         : IRequestHandler<ApplyDoctorLeaveCommand, Result<Guid>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<ApplyDoctorLeaveCommandHandler> _logger;

        public ApplyDoctorLeaveCommandHandler(
            AppointmentDbContext context,
            ILogger<ApplyDoctorLeaveCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(
            ApplyDoctorLeaveCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Create leave record
                var leave = new Domain.Entities.DoctorLeave
                {
                    Id = Guid.NewGuid(),
                    DoctorId = request.DoctorId,
                    Type = request.LeaveType,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Reason = request.Reason,
                    Status = Staff.Domain.Enums.LeaveStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.DoctorLeaves.Add(leave);

                // If auto-approve or reschedule existing appointments
                if (request.RescheduleExistingAppointments)
                {
                    var affectedAppointments = await _context.Appointments
                        .Where(a => a.DoctorId == request.DoctorId
                            && a.AppointmentDate.Date >= request.StartDate.Date
                            && a.AppointmentDate.Date <= request.EndDate.Date
                            && (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed))
                        .ToListAsync(cancellationToken);

                    foreach (var appointment in affectedAppointments)
                    {
                        appointment.Status = AppointmentStatus.Rescheduled;
                        appointment.Notes = $"Doctor on leave from {request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}";
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Leave applied for doctor {DoctorId} from {StartDate} to {EndDate}",
                    request.DoctorId, request.StartDate, request.EndDate);

                return Result<Guid>.Success(leave.Id, "Leave application submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying doctor leave");
                return Result<Guid>.Failure("An error occurred while applying for leave");
            }
        }
    }

}
