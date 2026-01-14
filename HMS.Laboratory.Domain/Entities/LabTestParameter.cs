namespace HMS.Laboratory.Domain.Entities
{
    public class LabTestParameter
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public string ParameterCode { get; set; }
        public string ParameterName { get; set; }
        public string? ReferenceRange { get; set; }
        public string? Units { get; set; }
        public string? Method { get; set; }
        public bool IsCritical { get; set; }
        public string? CriticalRange { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public LabTest Test { get; set; }
    }
}
