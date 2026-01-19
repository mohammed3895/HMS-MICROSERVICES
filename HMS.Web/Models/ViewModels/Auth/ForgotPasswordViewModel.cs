using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}
