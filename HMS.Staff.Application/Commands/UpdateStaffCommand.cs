using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class UpdateStaffCommand : IRequest<Result<bool>>
    {
        public Guid StaffId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhoneNumber { get; set; }
        public string Department { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string Position { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public Guid UpdatedBy { get; set; }
    }
}
