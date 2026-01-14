namespace HMS.Staff.Domain.Entities
{
    public class PatientAdmission
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid WardId { get; set; }
        public Guid? BedId { get; set; }
        public Guid? AdmittingPhysicianId { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string AdmissionReason { get; set; } = string.Empty;
        public string? DischargeNotes { get; set; }
        public string Status { get; set; } = "Active"; // Active, Discharged, Transferred
        public int? RoomNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        //public virtual Patient? Patient { get; set; }
        public virtual Ward? Ward { get; set; }
        public virtual Bed? Bed { get; set; }
        public virtual Staff? AdmittingPhysician { get; set; }
        public virtual ICollection<NurseNote> NurseNotes { get; set; } = [];
    }
}
