using HMS.Appointment.Application.Commands;
using HMS.Appointment.Application.DTOs;
using HMS.Appointment.Domain.Enums;
using HMS.Appointment.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HMS.Appointment.Application.Handlers
{
    public class CreateAppointmentCommandHandler
        : IRequestHandler<CreateAppointmentCommand, Result<CreateAppointmentResponse>>
    {
        private readonly AppointmentDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateAppointmentCommandHandler> _logger;

        public CreateAppointmentCommandHandler(
            AppointmentDbContext context,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<CreateAppointmentCommandHandler> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result<CreateAppointmentResponse>> Handle(
            CreateAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validate patient exists (call Patient Service)
                var patientClient = _httpClientFactory.CreateClient("PatientService");
                var patientResponse = await patientClient.GetAsync(
                    $"/api/patients/{request.PatientId}", cancellationToken);

                if (!patientResponse.IsSuccessStatusCode)
                {
                    return Result<CreateAppointmentResponse>.Failure(
                        "Patient not found");
                }

                var patientData = await patientResponse.Content.ReadFromJsonAsync<PatientDto>(
                    cancellationToken: cancellationToken);

                // 2. Validate doctor exists and get details (call Doctor Service)
                var doctorClient = _httpClientFactory.CreateClient("DoctorService");
                var doctorResponse = await doctorClient.GetAsync(
                    $"/api/doctors/{request.DoctorId}", cancellationToken);

                if (!doctorResponse.IsSuccessStatusCode)
                {
                    return Result<CreateAppointmentResponse>.Failure(
                        "Doctor not found");
                }

                var doctorData = await doctorResponse.Content.ReadFromJsonAsync<DoctorDto>(
                    cancellationToken: cancellationToken);

                // 3. Check time slot availability
                var endTime = request.StartTime.Add(TimeSpan.FromMinutes(request.DurationMinutes));

                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.DoctorId == request.DoctorId
                        && a.AppointmentDate.Date == request.AppointmentDate.Date
                        && a.Status != AppointmentStatus.Cancelled
                        && a.Status != AppointmentStatus.NoShow
                        && ((a.StartTime < endTime && a.EndTime > request.StartTime)))
                    .FirstOrDefaultAsync(cancellationToken);

                if (conflictingAppointment != null)
                {
                    return Result<CreateAppointmentResponse>.Failure(
                        "Time slot is not available");
                }

                // 4. Check if appointment is within doctor's schedule
                var dayOfWeek = request.AppointmentDate.DayOfWeek;
                var schedule = await _context.DoctorSchedules
                    .Where(s => s.DoctorId == request.DoctorId
                        && s.DayOfWeek == dayOfWeek
                        && s.IsActive
                        && s.StartTime <= request.StartTime
                        && s.EndTime >= endTime)
                    .FirstOrDefaultAsync(cancellationToken);

                if (schedule == null)
                {
                    return Result<CreateAppointmentResponse>.Failure(
                        "Appointment time is outside doctor's working hours");
                }

                // 5. Check for doctor leave
                var isOnLeave = await _context.DoctorLeaves
                    .AnyAsync(l => l.DoctorId == request.DoctorId
                        && l.StartDate.Date <= request.AppointmentDate.Date
                        && l.EndDate.Date >= request.AppointmentDate.Date
                        && l.Status.ToString() == LeaveStatus.Approved.ToString(),
                        cancellationToken);

                if (isOnLeave)
                {
                    return Result<CreateAppointmentResponse>.Failure(
                        "Doctor is on leave on the selected date");
                }

                // 6. Generate appointment number
                var appointmentCount = await _context.Appointments
                    .CountAsync(cancellationToken);
                var appointmentNumber = $"APT-{DateTime.UtcNow.Year}-{(appointmentCount + 1):D6}";

                // 7. Create appointment
                var appointment = new Domain.Entities.Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentNumber = appointmentNumber,
                    PatientId = request.PatientId,
                    PatientName = patientData?.FullName ?? "Unknown",
                    PatientPhone = patientData?.PhoneNumber ?? "",
                    PatientEmail = patientData?.Email ?? "",
                    DoctorId = request.DoctorId,
                    DoctorName = doctorData?.FullName ?? "Unknown",
                    Department = doctorData?.Department ?? "",
                    Specialization = doctorData?.Specialization ?? "",
                    AppointmentDate = request.AppointmentDate,
                    StartTime = request.StartTime,
                    EndTime = endTime,
                    DurationMinutes = request.DurationMinutes,
                    Type = Enum.Parse<AppointmentType>(request.AppointmentType),
                    Priority = Enum.Parse<AppointmentPriority>(request.Priority),
                    Status = AppointmentStatus.Scheduled,
                    ChiefComplaint = request.ChiefComplaint,
                    Notes = request.Notes,
                    IsInsuranceCovered = request.IsInsuranceCovered,
                    InsuranceProvider = request.InsuranceProvider,
                    InsurancePolicyNumber = request.InsurancePolicyNumber,
                    ConsultationFee = doctorData?.ConsultationFee ?? 0,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);

                // 8. Create appointment history entry
                var history = new Domain.Entities.AppointmentHistory
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Action = "Created",
                    NewValue = $"Appointment scheduled for {request.AppointmentDate:yyyy-MM-dd} at {request.StartTime}",
                    PerformedBy = request.CreatedBy,
                    PerformedByName = "System", // Would get from user service
                    PerformedAt = DateTime.UtcNow
                };

                _context.AppointmentHistories.Add(history);

                // 9. Schedule reminders
                var reminder24h = new Domain.Entities.AppointmentReminder
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Type = ReminderType.SMS,
                    Timing = ReminderTiming.TwentyFourHours,
                    ScheduledFor = request.AppointmentDate.Add(request.StartTime).AddHours(-24),
                    IsSent = false
                };

                var reminder2h = new Domain.Entities.AppointmentReminder
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    Type = ReminderType.Email,
                    Timing = ReminderTiming.TwoHours,
                    ScheduledFor = request.AppointmentDate.Add(request.StartTime).AddHours(-2),
                    IsSent = false
                };

                _context.AppointmentReminders.AddRange(reminder24h, reminder2h);

                await _context.SaveChangesAsync(cancellationToken);

                // 10. Send confirmation notification (async)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var notificationClient = _httpClientFactory.CreateClient("NotificationService");
                        await notificationClient.PostAsJsonAsync("/api/notifications/appointment-confirmation",
                            new
                            {
                                AppointmentId = appointment.Id,
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
                            });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send appointment confirmation");
                    }
                }, cancellationToken);

                var response = new CreateAppointmentResponse
                {
                    AppointmentId = appointment.Id,
                    AppointmentNumber = appointment.AppointmentNumber,
                    AppointmentDate = appointment.AppointmentDate,
                    StartTime = appointment.StartTime,
                    EndTime = appointment.EndTime,
                    DoctorName = appointment.DoctorName,
                    Department = appointment.Department,
                    RoomNumber = appointment.RoomNumber,
                    ConsultationFee = appointment.ConsultationFee
                };

                _logger.LogInformation(
                    "Appointment created: {AppointmentNumber} for patient {PatientId} with doctor {DoctorId}",
                    appointmentNumber, request.PatientId, request.DoctorId);

                return Result<CreateAppointmentResponse>.Success(
                    response, "Appointment created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return Result<CreateAppointmentResponse>.Failure(
                    "An error occurred while creating the appointment");
            }
        }
    }

    // DTOs for external service calls
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class DoctorDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public decimal ConsultationFee { get; set; }
    }
}