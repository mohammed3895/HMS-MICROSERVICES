namespace HMS.Authentication.Application.DTOs.Users
{
    public class CreateUserResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
