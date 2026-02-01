using HMS.Appointment.Application.DTOs;
using HMS.Appointment.Application.Queries;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Application.Handlers
{
    public class GetAvailableTimeSlotsQueryHandler
        : IRequestHandler<GetAvailableTimeSlotsQuery, Result<List<TimeSlotDto>>>
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<GetAvailableTimeSlotsQueryHandler> _logger;

        public GetAvailableTimeSlotsQueryHandler(
            AppointmentDbContext context,
            ILogger<GetAvailableTimeSlotsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<TimeSlotDto>>> Handle(
            GetAvailableTimeSlotsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var dayOfWeek = request.Date.DayOfWeek;

                // Get doctor's schedule for the requested day
                var schedule = await _context.DoctorSchedules
                    .Where(s => s.DoctorId == request.DoctorId
                        && s.DayOfWeek == dayOfWeek
                        && s.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (schedule == null)
                {
                    return Result<List<TimeSlotDto>>.Success(
                        new List<TimeSlotDto>(),
                        "Doctor is not available on this day");
                }

                // Check if doctor is on leave
                var isOnLeave = await _context.DoctorLeaves
                    .AnyAsync(l => l.DoctorId == request.DoctorId
                        && l.StartDate.Date <= request.Date.Date
                        && l.EndDate.Date >= request.Date.Date
                        && l.Status == Staff.Domain.Enums.LeaveStatus.Approved,
                        cancellationToken);

                if (isOnLeave)
                {
                    return Result<List<TimeSlotDto>>.Success(
                        new List<TimeSlotDto>(),
                        "Doctor is on leave on this date");
                }

                // Get existing appointments for the day
                var existingAppointments = await _context.Appointments
                    .Where(a => a.DoctorId == request.DoctorId
                        && a.AppointmentDate.Date == request.Date.Date
                        && a.Status != AppointmentStatus.Cancelled
                        && a.Status != AppointmentStatus.NoShow)
                    .Select(a => new { a.StartTime, a.EndTime })
                    .ToListAsync(cancellationToken);

                // Generate time slots
                var timeSlots = new List<TimeSlotDto>();
                var currentTime = schedule.StartTime;
                var slotDuration = TimeSpan.FromMinutes(schedule.SlotDurationMinutes);

                while (currentTime.Add(slotDuration) <= schedule.EndTime)
                {
                    var endTime = currentTime.Add(slotDuration);

                    // Check if slot overlaps with any existing appointment
                    var isAvailable = !existingAppointments.Any(a =>
                        (currentTime < a.EndTime && endTime > a.StartTime));

                    timeSlots.Add(new TimeSlotDto
                    {
                        StartTime = currentTime,
                        EndTime = endTime,
                        IsAvailable = isAvailable,
                        AvailableSlots = isAvailable ? 1 : 0
                    });

                    currentTime = endTime;
                }

                _logger.LogInformation(
                    "Retrieved {Count} time slots for doctor {DoctorId} on {Date}",
                    timeSlots.Count, request.DoctorId, request.Date.Date);

                return Result<List<TimeSlotDto>>.Success(
                    timeSlots,
                    "Time slots retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available time slots");
                return Result<List<TimeSlotDto>>.Failure(
                    "An error occurred while retrieving time slots");
            }
        }
    }
}