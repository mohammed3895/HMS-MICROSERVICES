namespace HMS.Laboratory.Domain.Enums
{
    public enum ReportDeliveryMethod
    {
        Portal = 1,        // Online patient portal
        Email = 2,         // Email to patient/doctor
        Print = 3,         // Printed copy
        Fax = 4,           // Fax transmission
        HL7 = 5,           // HL7 interface to EHR
        API = 6,           // API delivery
        MobileApp = 7,     // Mobile application
        InPerson = 8       // In-person pickup
    }
}
