using HMS.Pharamcy.Dimain.Enums;

namespace HMS.Pharamcy.Dimain.Entities
{
    public class Prescription
    {
        public Guid Id { get; set; }
        public string PrescriptionNumber { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime PrescribedDate { get; set; }
        public DateTime? DispensedDate { get; set; }
        public PrescriptionStatus Status { get; set; }
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public Guid? DispensedBy { get; set; }
        public ICollection<PrescriptionItem> Items { get; set; }
    }
}
