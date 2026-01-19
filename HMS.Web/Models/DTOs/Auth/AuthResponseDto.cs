namespace HMS.Web.Models.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public AuthDataDto Data { get; set; }
    }
}
