namespace HMS.Web.Models.DTOs.Patient
{
    public class MedicalRecordDto
    {
        public Guid RecordId { get; set; }
        public Guid PatientId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
