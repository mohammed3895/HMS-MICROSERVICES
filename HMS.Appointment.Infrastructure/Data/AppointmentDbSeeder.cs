using HMS.Appointment.Domain.Entities;
using HMS.Appointment.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Appointment.Infrastructure.Data
{
    public static class AppointmentDbSeeder
    {
        // Predefined GUIDs for consistency
        private static readonly Guid Doctor1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid Doctor2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid Doctor3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid Doctor4Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

        private static readonly Guid Patient1Id = Guid.Parse("a1111111-1111-1111-1111-111111111111");
        private static readonly Guid Patient2Id = Guid.Parse("a2222222-2222-2222-2222-222222222222");
        private static readonly Guid Patient3Id = Guid.Parse("a3333333-3333-3333-3333-333333333333");
        private static readonly Guid Patient4Id = Guid.Parse("a4444444-4444-4444-4444-444444444444");
        private static readonly Guid Patient5Id = Guid.Parse("a5555555-5555-5555-5555-555555555555");

        public static async Task SeedAsync(AppointmentDbContext context, ILogger logger)
        {
            try
            {
                // Check if data already exists
                if (await context.DoctorSchedules.AnyAsync())
                {
                    logger.LogInformation("Database already seeded. Skipping seed data.");
                    return;
                }

                logger.LogInformation("Starting to seed appointment database...");

                await SeedDoctorSchedules(context);
                await SeedDoctorLeaves(context);
                await SeedAppointments(context);
                await SeedWaitlistEntries(context);
                await SeedTimeSlots(context);

                await context.SaveChangesAsync();

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static async Task SeedDoctorSchedules(AppointmentDbContext context)
        {
            var effectiveFrom = DateTime.UtcNow.Date.AddDays(-30); // Effective from 30 days ago

            var schedules = new List<DoctorSchedule>
            {
                // Dr. Sarah Johnson - Cardiologist (Monday-Friday)
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor1Id,
                    DoctorName = "Dr. Sarah Johnson",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    SlotDurationMinutes = 30,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor1Id,
                    DoctorName = "Dr. Sarah Johnson",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    SlotDurationMinutes = 30,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor1Id,
                    DoctorName = "Dr. Sarah Johnson",
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    SlotDurationMinutes = 30,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor1Id,
                    DoctorName = "Dr. Sarah Johnson",
                    DayOfWeek = DayOfWeek.Thursday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    SlotDurationMinutes = 30,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor1Id,
                    DoctorName = "Dr. Sarah Johnson",
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0),
                    SlotDurationMinutes = 30,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },

                // Dr. Michael Chen - Pediatrician (Monday-Saturday)
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor2Id,
                    DoctorName = "Dr. Michael Chen",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    SlotDurationMinutes = 20,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor2Id,
                    DoctorName = "Dr. Michael Chen",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    SlotDurationMinutes = 20,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor2Id,
                    DoctorName = "Dr. Michael Chen",
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    SlotDurationMinutes = 20,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor2Id,
                    DoctorName = "Dr. Michael Chen",
                    DayOfWeek = DayOfWeek.Thursday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    SlotDurationMinutes = 20,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor2Id,
                    DoctorName = "Dr. Michael Chen",
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    SlotDurationMinutes = 20,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor2Id,
                    DoctorName = "Dr. Michael Chen",
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(13, 0, 0),
                    SlotDurationMinutes = 20,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },

                // Dr. Emily Rodriguez - Orthopedic Surgeon (Monday-Friday)
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor3Id,
                    DoctorName = "Dr. Emily Rodriguez",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    SlotDurationMinutes = 45,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor3Id,
                    DoctorName = "Dr. Emily Rodriguez",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    SlotDurationMinutes = 45,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor3Id,
                    DoctorName = "Dr. Emily Rodriguez",
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    SlotDurationMinutes = 45,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor3Id,
                    DoctorName = "Dr. Emily Rodriguez",
                    DayOfWeek = DayOfWeek.Thursday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    SlotDurationMinutes = 45,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor3Id,
                    DoctorName = "Dr. Emily Rodriguez",
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    SlotDurationMinutes = 45,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },

                // Dr. James Wilson - General Practitioner (Monday-Saturday)
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor4Id,
                    DoctorName = "Dr. James Wilson",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(17, 30, 0),
                    SlotDurationMinutes = 15,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor4Id,
                    DoctorName = "Dr. James Wilson",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(17, 30, 0),
                    SlotDurationMinutes = 15,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor4Id,
                    DoctorName = "Dr. James Wilson",
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(17, 30, 0),
                    SlotDurationMinutes = 15,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor4Id,
                    DoctorName = "Dr. James Wilson",
                    DayOfWeek = DayOfWeek.Thursday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(17, 30, 0),
                    SlotDurationMinutes = 15,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor4Id,
                    DoctorName = "Dr. James Wilson",
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(17, 30, 0),
                    SlotDurationMinutes = 15,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                },
                new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor4Id,
                    DoctorName = "Dr. James Wilson",
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    SlotDurationMinutes = 15,
                    IsActive = true,
                    EffectiveFrom = effectiveFrom,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.DoctorSchedules.AddRangeAsync(schedules);
        }

        private static async Task SeedDoctorLeaves(AppointmentDbContext context)
        {
            var baseDate = DateTime.UtcNow.Date;
            var leaves = new List<DoctorLeave>
            {
                new DoctorLeave
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor2Id,
                    DoctorName = "Dr. Michael Chen",
                    Type = Staff.Domain.Enums.LeaveType.Paternity,
                    StartDate = baseDate.AddDays(10),
                    EndDate = baseDate.AddDays(17),
                    Reason = "Annual family vacation",
                    Status = Staff.Domain.Enums.LeaveStatus.Approved,
                    CreatedAt = baseDate.AddDays(-15),
                    ApprovedBy = Guid.NewGuid(),
                    ApprovedAt = baseDate.AddDays(-14)
                },
                new DoctorLeave
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor3Id,
                    DoctorName = "Dr. Emily Rodriguez",
                    Type = Staff.Domain.Enums.LeaveType.Compensatory,
                    StartDate = baseDate.AddDays(20),
                    EndDate = baseDate.AddDays(22),
                    Reason = "Attending Orthopedic Surgery Conference",
                    Status = Staff.Domain.Enums.LeaveStatus.Pending,
                    CreatedAt = baseDate.AddDays(-5)
                }
            };

            await context.DoctorLeaves.AddRangeAsync(leaves);
        }

        private static async Task SeedAppointments(AppointmentDbContext context)
        {
            var baseDate = DateTime.UtcNow.Date;
            var appointments = new List<Domain.Entities.Appointment>();
            var histories = new List<AppointmentHistory>();
            var reminders = new List<AppointmentReminder>();

            // Appointment 1: Completed appointment from yesterday
            var apt1Id = Guid.NewGuid();
            var apt1 = new Domain.Entities.Appointment
            {
                Id = apt1Id,
                AppointmentNumber = "APT-2025-000001",
                PatientId = Patient1Id,
                PatientName = "John Smith",
                PatientPhone = "+1234567890",
                PatientEmail = "john.smith@email.com",
                DoctorId = Doctor1Id,
                DoctorName = "Dr. Sarah Johnson",
                Department = "Cardiology",
                Specialization = "Cardiologist",
                AppointmentDate = baseDate.AddDays(-1),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(10, 30, 0),
                DurationMinutes = 30,
                Type = AppointmentType.FollowUp,
                Priority = AppointmentPriority.Normal,
                Status = AppointmentStatus.Completed,
                ChiefComplaint = "Follow-up for hypertension management",
                Notes = "Patient reports improved blood pressure readings",
                ConsultationFee = 150.00m,
                IsInsuranceCovered = true,
                InsuranceProvider = "Blue Cross",
                InsurancePolicyNumber = "BC123456",
                CheckInTime = baseDate.AddDays(-1).Add(new TimeSpan(9, 55, 0)),
                ConsultationStartTime = baseDate.AddDays(-1).Add(new TimeSpan(10, 5, 0)),
                ConsultationEndTime = baseDate.AddDays(-1).Add(new TimeSpan(10, 28, 0)),
                RoomNumber = "201",
                CreatedBy = Guid.NewGuid(),
                CreatedAt = baseDate.AddDays(-7)
            };
            appointments.Add(apt1);

            histories.Add(new AppointmentHistory
            {
                Id = Guid.NewGuid(),
                AppointmentId = apt1Id,
                Action = "Created",
                NewValue = "Appointment scheduled",
                PerformedBy = Guid.NewGuid(),
                PerformedByName = "Admin",
                PerformedAt = baseDate.AddDays(-7)
            });

            // Appointment 2: Scheduled for today
            var apt2Id = Guid.NewGuid();
            var apt2 = new Domain.Entities.Appointment
            {
                Id = apt2Id,
                AppointmentNumber = "APT-2025-000002",
                PatientId = Patient2Id,
                PatientName = "Emma Davis",
                PatientPhone = "+1234567891",
                PatientEmail = "emma.davis@email.com",
                DoctorId = Doctor2Id,
                DoctorName = "Dr. Michael Chen",
                Department = "Pediatrics",
                Specialization = "Pediatrician",
                AppointmentDate = baseDate,
                StartTime = new TimeSpan(14, 0, 0),
                EndTime = new TimeSpan(14, 20, 0),
                DurationMinutes = 20,
                Type = AppointmentType.Procedure,
                Priority = AppointmentPriority.Normal,
                Status = AppointmentStatus.Confirmed,
                ChiefComplaint = "Annual checkup for 5-year-old child",
                Notes = "Vaccination review needed",
                ConsultationFee = 100.00m,
                IsInsuranceCovered = true,
                InsuranceProvider = "United Healthcare",
                InsurancePolicyNumber = "UH789012",
                CreatedBy = Guid.NewGuid(),
                CreatedAt = baseDate.AddDays(-3)
            };
            appointments.Add(apt2);

            reminders.AddRange(new[]
            {
                new AppointmentReminder
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = apt2Id,
                    Type = ReminderType.Email,
                    Timing = ReminderTiming.TwentyFourHours,
                    ScheduledFor = baseDate.Add(new TimeSpan(14, 0, 0)).AddHours(-24),
                    IsSent = true,
                    SentAt = baseDate.AddDays(-1)
                },
                new AppointmentReminder
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = apt2Id,
                    Type = ReminderType.SMS,
                    Timing = ReminderTiming.TwoHours,
                    ScheduledFor = baseDate.Add(new TimeSpan(14, 0, 0)).AddHours(-2),
                    IsSent = false
                }
            });

            // Appointment 3: Checked in, waiting
            var apt3Id = Guid.NewGuid();
            var apt3 = new Domain.Entities.Appointment
            {
                Id = apt3Id,
                AppointmentNumber = "APT-2025-000003",
                PatientId = Patient3Id,
                PatientName = "Robert Johnson",
                PatientPhone = "+1234567892",
                PatientEmail = "robert.j@email.com",
                DoctorId = Doctor3Id,
                DoctorName = "Dr. Emily Rodriguez",
                Department = "Orthopedics",
                Specialization = "Orthopedic Surgeon",
                AppointmentDate = baseDate,
                StartTime = new TimeSpan(11, 0, 0),
                EndTime = new TimeSpan(11, 45, 0),
                DurationMinutes = 45,
                Type = AppointmentType.Consultation,
                Priority = AppointmentPriority.Urgent,
                Status = AppointmentStatus.CheckedIn,
                ChiefComplaint = "Knee pain and swelling after sports injury",
                Notes = "X-ray results available",
                ConsultationFee = 200.00m,
                IsInsuranceCovered = false,
                CheckInTime = baseDate.Add(new TimeSpan(10, 45, 0)),
                CheckInMethod = "Kiosk",
                RoomNumber = "305",
                CreatedBy = Guid.NewGuid(),
                CreatedAt = baseDate.AddDays(-5)
            };
            appointments.Add(apt3);

            // Appointment 4: Future appointment (tomorrow)
            var apt4Id = Guid.NewGuid();
            var apt4 = new Domain.Entities.Appointment
            {
                Id = apt4Id,
                AppointmentNumber = "APT-2025-000004",
                PatientId = Patient4Id,
                PatientName = "Maria Garcia",
                PatientPhone = "+1234567893",
                PatientEmail = "maria.garcia@email.com",
                DoctorId = Doctor4Id,
                DoctorName = "Dr. James Wilson",
                Department = "General Medicine",
                Specialization = "General Practitioner",
                AppointmentDate = baseDate.AddDays(1),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 15, 0),
                DurationMinutes = 15,
                Type = AppointmentType.Consultation,
                Priority = AppointmentPriority.Normal,
                Status = AppointmentStatus.Scheduled,
                ChiefComplaint = "Persistent headaches",
                Notes = "Patient reports headaches for past week",
                ConsultationFee = 80.00m,
                IsInsuranceCovered = true,
                InsuranceProvider = "Aetna",
                InsurancePolicyNumber = "AE345678",
                CreatedBy = Guid.NewGuid(),
                CreatedAt = baseDate.AddDays(-2)
            };
            appointments.Add(apt4);

            // Appointment 5: Cancelled appointment
            var apt5Id = Guid.NewGuid();
            var apt5 = new Domain.Entities.Appointment
            {
                Id = apt5Id,
                AppointmentNumber = "APT-2025-000005",
                PatientId = Patient5Id,
                PatientName = "David Lee",
                PatientPhone = "+1234567894",
                PatientEmail = "david.lee@email.com",
                DoctorId = Doctor1Id,
                DoctorName = "Dr. Sarah Johnson",
                Department = "Cardiology",
                Specialization = "Cardiologist",
                AppointmentDate = baseDate.AddDays(2),
                StartTime = new TimeSpan(15, 0, 0),
                EndTime = new TimeSpan(15, 30, 0),
                DurationMinutes = 30,
                Type = AppointmentType.Consultation,
                Priority = AppointmentPriority.Normal,
                Status = AppointmentStatus.Cancelled,
                ChiefComplaint = "Chest pain evaluation",
                CancellationReason = "Patient recovered, no longer needed",
                CancelledBy = Patient5Id,
                CancelledAt = baseDate.AddHours(-2),
                ConsultationFee = 150.00m,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = baseDate.AddDays(-4)
            };
            appointments.Add(apt5);

            histories.Add(new AppointmentHistory
            {
                Id = Guid.NewGuid(),
                AppointmentId = apt5Id,
                Action = "Cancelled",
                OldValue = "Scheduled",
                NewValue = "Cancelled",
                Reason = "Patient recovered, no longer needed",
                PerformedBy = Patient5Id,
                PerformedByName = "David Lee",
                PerformedAt = baseDate.AddHours(-2)
            });

            // Appointment 6: In Progress
            var apt6Id = Guid.NewGuid();
            var apt6 = new Domain.Entities.Appointment
            {
                Id = apt6Id,
                AppointmentNumber = "APT-2025-000006",
                PatientId = Patient1Id,
                PatientName = "John Smith",
                PatientPhone = "+1234567890",
                PatientEmail = "john.smith@email.com",
                DoctorId = Doctor4Id,
                DoctorName = "Dr. James Wilson",
                Department = "General Medicine",
                Specialization = "General Practitioner",
                AppointmentDate = baseDate,
                StartTime = new TimeSpan(10, 30, 0),
                EndTime = new TimeSpan(10, 45, 0),
                DurationMinutes = 15,
                Type = AppointmentType.Consultation,
                Priority = AppointmentPriority.Urgent,
                Status = AppointmentStatus.InProgress,
                ChiefComplaint = "Flu-like symptoms",
                Notes = "Fever, cough, and body aches since yesterday",
                ConsultationFee = 80.00m,
                IsInsuranceCovered = true,
                InsuranceProvider = "Blue Cross",
                InsurancePolicyNumber = "BC123456",
                CheckInTime = baseDate.Add(new TimeSpan(10, 25, 0)),
                ConsultationStartTime = baseDate.Add(new TimeSpan(10, 32, 0)),
                RoomNumber = "102",
                CreatedBy = Guid.NewGuid(),
                CreatedAt = baseDate.AddHours(-1)
            };
            appointments.Add(apt6);

            await context.Appointments.AddRangeAsync(appointments);
            await context.AppointmentHistories.AddRangeAsync(histories);
            await context.AppointmentReminders.AddRangeAsync(reminders);
        }

        private static async Task SeedWaitlistEntries(AppointmentDbContext context)
        {
            var baseDate = DateTime.UtcNow.Date;
            var waitlistEntries = new List<WaitlistEntry>
            {
                new WaitlistEntry
                {
                    Id = Guid.NewGuid(),
                    PatientId = Patient2Id,
                    PatientName = "Emma Davis",
                    PatientPhone = "+1234567891",
                    PatientEmail = "emma.davis@email.com",
                    DoctorId = Doctor1Id,
                    DoctorName = "Dr. Sarah Johnson",
                    PreferredDate = baseDate.AddDays(3),
                    PreferredStartTime = new TimeSpan(9, 0, 0),
                    PreferredEndTime = new TimeSpan(12, 0, 0),
                    Priority = WaitlistPriority.Medium,
                    Status = WaitlistStatus.Active,
                    Notes = "Prefers morning appointments",
                    CreatedAt = baseDate.AddDays(-2),
                    ExpiresAt = baseDate.AddDays(10)
                },
                new WaitlistEntry
                {
                    Id = Guid.NewGuid(),
                    PatientId = Patient4Id,
                    PatientName = "Maria Garcia",
                    PatientPhone = "+1234567893",
                    PatientEmail = "maria.garcia@email.com",
                    DoctorId = Doctor3Id,
                    DoctorName = "Dr. Emily Rodriguez",
                    PreferredDate = baseDate.AddDays(5),
                    PreferredStartTime = new TimeSpan(14, 0, 0),
                    PreferredEndTime = new TimeSpan(17, 0, 0),
                    Priority = WaitlistPriority.High,
                    Status = WaitlistStatus.Active,
                    Notes = "Patient needs orthopedic consultation urgently",
                    CreatedAt = baseDate.AddDays(-1),
                    ExpiresAt = baseDate.AddDays(12)
                }
            };

            await context.WaitlistEntries.AddRangeAsync(waitlistEntries);
        }

        private static async Task SeedTimeSlots(AppointmentDbContext context)
        {
            var baseDate = DateTime.UtcNow.Date;
            var timeSlots = new List<TimeSlot>();

            // Create available time slots for Dr. Sarah Johnson for tomorrow
            var tomorrow = baseDate.AddDays(1);
            var slotTime = new TimeSpan(9, 0, 0);
            var endTime = new TimeSpan(17, 0, 0);

            while (slotTime < endTime)
            {
                timeSlots.Add(new TimeSlot
                {
                    Id = Guid.NewGuid(),
                    DoctorId = Doctor1Id,
                    Date = tomorrow,
                    StartTime = slotTime,
                    EndTime = slotTime.Add(TimeSpan.FromMinutes(30)),
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                });

                slotTime = slotTime.Add(TimeSpan.FromMinutes(30));
            }

            // Block one slot for lunch
            var lunchSlot = timeSlots.FirstOrDefault(s => s.StartTime == new TimeSpan(12, 0, 0));
            if (lunchSlot != null)
            {
                lunchSlot.IsAvailable = false;
                lunchSlot.IsBlocked = true;
                lunchSlot.BlockReason = "Lunch break";
            }

            await context.TimeSlots.AddRangeAsync(timeSlots);
        }
    }
}