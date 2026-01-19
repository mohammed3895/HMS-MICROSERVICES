using Microsoft.AspNetCore.Authorization;

namespace HMS.Authentication.Infrastructure.Authorization.Polices
{
    public static class AuthorizationPolicyConfiguration
    {
        public static void ConfigurePolicies(AuthorizationOptions options)
        {
            // Role-based policies
            options.AddPolicy(PolicyNames.AdminOnly, policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy(PolicyNames.ManagerOnly, policy =>
                policy.RequireRole("Manager"));

            options.AddPolicy(PolicyNames.AdminOrManager, policy =>
                policy.RequireRole("Admin", "Manager"));

            options.AddPolicy(PolicyNames.DoctorOnly, policy =>
                policy.RequireRole("Doctor"));

            options.AddPolicy(PolicyNames.NurseOnly, policy =>
                policy.RequireRole("Nurse"));

            options.AddPolicy(PolicyNames.PharmacistOnly, policy =>
                policy.RequireRole("Pharmacist"));

            options.AddPolicy(PolicyNames.LabTechnicianOnly, policy =>
                policy.RequireRole("LabTechnician"));

            options.AddPolicy(PolicyNames.ReceptionistOnly, policy =>
                policy.RequireRole("Receptionist"));

            options.AddPolicy(PolicyNames.PatientOnly, policy =>
                policy.RequireRole("Patient"));

            // Combined policies
            options.AddPolicy(PolicyNames.MedicalStaff, policy =>
                policy.RequireRole("Doctor", "Nurse"));

            options.AddPolicy(PolicyNames.StaffOnly, policy =>
                policy.RequireRole("Admin", "Manager", "Doctor", "Nurse", "Pharmacist", "LabTechnician", "Receptionist"));

            // Status-based policies
            options.AddPolicy(PolicyNames.EmailConfirmed, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new EmailConfirmedRequirement());
            });

            options.AddPolicy(PolicyNames.ActiveAccount, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new ActiveAccountRequirement());
            });

            options.AddPolicy(PolicyNames.VerifiedAccount, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(
                    new EmailConfirmedRequirement(),
                    new ActiveAccountRequirement());
            });
        }
    }
}
