using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models.ViewModels.Auth
{
    public class VerifyEmailViewModel
    {
        public Guid UserId { get; set; }

        public string? Email { get; set; }

        [Required(ErrorMessage = "Verification code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be 6 digits")]
        public string OtpCode { get; set; } = string.Empty;

        public string? Token { get; set; }
    }
}
