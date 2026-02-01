using HMS.Appointment.Application.Commands;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class AddToWaitlistCommandHandler
         : IRequestHandler<AddToWaitlistCommand, Result<Guid>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<AddToWaitlistCommandHandler> _logger;

        public AddToWaitlistCommandHandler(
            AppointmentDbContext context,
            ILogger<AddToWaitlistCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(
            AddToWaitlistCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var waitlistEntry = new Domain.Entities.WaitlistEntry
                {
                    Id = Guid.NewGuid(),
                    PatientId = request.PatientId,
                    DoctorId = request.DoctorId,
                    PreferredDate = request.PreferredDate,
                    PreferredStartTime = request.PreferredStartTime,
                    PreferredEndTime = request.PreferredEndTime,
                    Priority = request.Priority,
                    Status = WaitlistStatus.Active,
                    Notes = request.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                _context.WaitlistEntries.Add(waitlistEntry);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Patient {PatientId} added to waitlist for doctor {DoctorId}",
                    request.PatientId, request.DoctorId);

                return Result<Guid>.Success(waitlistEntry.Id, "Added to waitlist successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to waitlist");
                return Result<Guid>.Failure("An error occurred while adding to waitlist");
            }
        }
    }

}
