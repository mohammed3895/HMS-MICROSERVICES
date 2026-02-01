namespace HMS.Staff.Application.DTOs
{
    public class UpdateUserBasicInfoRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
