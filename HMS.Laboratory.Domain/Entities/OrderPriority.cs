namespace HMS.Laboratory.Domain.Entities
{
    public enum OrderPriority
    {
        Routine = 1,      // Normal priority, 24-48 hour turnaround
        Urgent = 2,       // Urgent, 4-6 hour turnaround
        Stat = 3,         // Immediate, 1-2 hour turnaround
        ASAP = 4,         // As soon as possible, 2-4 hour turnaround
        Timed = 5,        // Specific time requirement
        Fasting = 6,      // Requires fasting
        PreOp = 7,        // Pre-operative testing
        PostOp = 8,       // Post-operative testing
        Emergency = 9,    // Emergency department
        Critical = 10,    // Critical care/ICU
        Newborn = 11,     // Newborn screening
        Outpatient = 12,  // Outpatient testing
        Inpatient = 13    // Inpatient testing
    }
}
