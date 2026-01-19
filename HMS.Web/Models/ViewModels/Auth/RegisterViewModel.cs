using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?\"":{}|<>]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "First name can only contain letters, spaces, hyphens, and apostrophes")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Last name can only contain letters, spaces, hyphens, and apostrophes")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Use E.164 format (e.g., +1234567890)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(50, ErrorMessage = "National ID cannot exceed 50 characters")]
        public string? NationalId { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string? State { get; set; }

        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string? Country { get; set; }

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string? PostalCode { get; set; }
    }
}
