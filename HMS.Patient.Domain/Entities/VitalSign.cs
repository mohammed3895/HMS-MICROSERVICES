namespace HMS.Patient.Domain.Entities
{
    public class VitalSign
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime RecordedAt { get; set; }
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
        public string? BloodPressure { get; set; }
        public decimal? OxygenSaturation { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Bmi { get; set; }
        public Guid? RecordedBy { get; set; }

        public Patient Patient { get; set; }
    }
}
