namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class OtpVerificationRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
