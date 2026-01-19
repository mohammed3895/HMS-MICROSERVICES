namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class ConfirmEmailResponse
    {
        public bool EmailConfirmed { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
