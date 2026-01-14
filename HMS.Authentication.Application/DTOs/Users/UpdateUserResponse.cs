namespace HMS.Authentication.Application.DTOs.Users
{
    public class UpdateUserResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
