using HMS.Authentication.Infrastructure.Authorization.Polices;
using Microsoft.AspNetCore.Authorization;

namespace HMS.Authentication.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireMedicalStaffAttribute : AuthorizeAttribute
    {
        public RequireMedicalStaffAttribute()
        {
            Policy = PolicyNames.MedicalStaff;
        }
    }
}
