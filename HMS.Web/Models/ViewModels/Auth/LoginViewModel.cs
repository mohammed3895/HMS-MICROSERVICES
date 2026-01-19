using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? DeviceId { get; set; }
        public string? OtpCode { get; set; }
        public bool RememberMe { get; set; }
    }
}
