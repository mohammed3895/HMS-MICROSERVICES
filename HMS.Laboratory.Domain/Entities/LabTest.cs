using HospitalManagement.Laboratory.API.Enums;

namespace HMS.Laboratory.Domain.Entities
{
    public class LabTest
    {
        public Guid Id { get; set; }
        public string TestCode { get; set; } // Laboratory test code
        public string Name { get; set; }
        public string? ShortName { get; set; }
        public TestCategory Category { get; set; }
        public string? SubCategory { get; set; }
        public string? Description { get; set; }

        // Sample Requirements
        public SampleType SampleType { get; set; }
        public string? SampleVolume { get; set; }
        public string? ContainerType { get; set; }
        public string? StorageInstructions { get; set; }
        public string? TransportInstructions { get; set; }

        // Testing Details
        public string? Methodology { get; set; }
        public string? Equipment { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public string? TurnaroundTime { get; set; } // Routine: 24h, Urgent: 4h

        // Reference Ranges
        public string? ReferenceRangeMale { get; set; }
        public string? ReferenceRangeFemale { get; set; }
        public string? ReferenceRangeChild { get; set; }
        public string? ReferenceRangeNewborn { get; set; }
        public string? CriticalRange { get; set; }
        public string? Units { get; set; }

        // Pricing
        public decimal Price { get; set; }
        public decimal InsurancePrice { get; set; }
        public decimal GovernmentPrice { get; set; }

        // Status
        public bool IsActive { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Additional Info
        public string? PreparationInstructions { get; set; }
        public string? InterferingSubstances { get; set; }
        public string? ClinicalSignificance { get; set; }
        public string? CPTCode { get; set; } // CPT billing code
        public string? LOINCCode { get; set; } // LOINC code

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<LabTestParameter> Parameters { get; set; } = new List<LabTestParameter>();
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
    }
}