
using HMS.Appointment.Domain.Enums;

namespace HMS.Appointment.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public string AppointmentNumber { get; set; } = string.Empty; // APT-2024-000001

        // Patient Information
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;

        // Doctor Information
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        // Appointment Details
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }

        // Type and Priority
        public AppointmentType Type { get; set; } // Consultation, Follow-up, Procedure, Emergency
        public AppointmentPriority Priority { get; set; } // Routine, Urgent, Emergency

        // Status Management
        public AppointmentStatus Status { get; set; } // Scheduled, Confirmed, CheckedIn, InProgress, Completed, Cancelled, NoShow
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public Guid? CancelledBy { get; set; }

        // Check-in Details
        public DateTime? CheckInTime { get; set; }
        public string? CheckInMethod { get; set; } // Kiosk, Reception, Mobile

        // Consultation Details
        public DateTime? ConsultationStartTime { get; set; }
        public DateTime? ConsultationEndTime { get; set; }
        public string? ChiefComplaint { get; set; }
        public string? Notes { get; set; }

        // Room and Location
        public string? RoomNumber { get; set; }
        public string? Floor { get; set; }
        public string? Building { get; set; }

        // Follow-up and Referral
        public bool RequiresFollowUp { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public Guid? ReferredToDoctor { get; set; }
        public string? ReferralNotes { get; set; }

        // Notifications
        public bool ReminderSent { get; set; }
        public DateTime? ReminderSentAt { get; set; }
        public bool ConfirmationSent { get; set; }
        public DateTime? ConfirmationSentAt { get; set; }

        // Billing
        public decimal ConsultationFee { get; set; }
        public bool IsPaid { get; set; }
        public Guid? BillingId { get; set; }

        // Insurance
        public bool IsInsuranceCovered { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }

        // Audit Trail
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relationships
        public virtual ICollection<AppointmentHistory> History { get; set; } = new List<AppointmentHistory>();
        public virtual ICollection<AppointmentReminder> Reminders { get; set; } = new List<AppointmentReminder>();
        public virtual WaitlistEntry? WaitlistEntry { get; set; }
    }
}
