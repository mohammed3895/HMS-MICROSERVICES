namespace HMS.Authentication.Infrastructure.Authorization.Polices
{
    public static class PolicyNames
    {
        // Role-based policies
        public const string AdminOnly = "AdminOnly";
        public const string ManagerOnly = "ManagerOnly";
        public const string AdminOrManager = "AdminOrManager";
        public const string DoctorOnly = "DoctorOnly";
        public const string NurseOnly = "NurseOnly";
        public const string PharmacistOnly = "PharmacistOnly";
        public const string LabTechnicianOnly = "LabTechnicianOnly";
        public const string ReceptionistOnly = "ReceptionistOnly";
        public const string PatientOnly = "PatientOnly";

        // Combined policies
        public const string MedicalStaff = "MedicalStaff"; // Doctor + Nurse
        public const string StaffOnly = "StaffOnly"; // All staff except patients

        // Status-based policies
        public const string EmailConfirmed = "EmailConfirmed";
        public const string ActiveAccount = "ActiveAccount";
        public const string VerifiedAccount = "VerifiedAccount"; // Email confirmed + Active
    }
}
