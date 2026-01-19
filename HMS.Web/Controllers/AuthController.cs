using HMS.Web.Interfaces;
using HMS.Web.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authService.LoginAsync(model);

                if (result.IsSuccess)
                {
                    if (result.Data?.RequiresEmailVerification == true)
                    {
                        TempData["UserId"] = result.Data.UserId;
                        TempData["Info"] = "Please verify your email to continue.";
                        return RedirectToAction(nameof(VerifyEmail));
                    }

                    if (result.Data?.RequiresTwoFactor == true)
                    {
                        TempData["UserId"] = result.Data.UserId;
                        TempData["TwoFactorToken"] = result.Data.TwoFactorToken;
                        TempData["Info"] = "Two-factor authentication required.";
                        return RedirectToAction(nameof(VerifyTwoFactor));
                    }

                    TempData["Success"] = "Welcome back! You've successfully signed in.";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Dashboard");
                }

                _logger.LogWarning("Failed login attempt for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, result.Message ?? "Invalid credentials.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login exception for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var model = new RegisterViewModel
            {
                DateOfBirth = DateTime.Now.AddYears(-18)
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var isStrongPassword = await _authService.ValidatePasswordStrength(model.Password);
                if (!isStrongPassword)
                {
                    ModelState.AddModelError("Password",
                        "Password must contain uppercase, lowercase, number, and special character.");
                    return View(model);
                }

                var result = await _authService.RegisterAsync(model);

                if (result.IsSuccess)
                {
                    if (result.Data?.EmailConfirmationRequired == true)
                    {
                        TempData["UserId"] = result.Data.UserId;
                        TempData["Email"] = result.Data.Email;
                        TempData["Success"] = "Registration successful! Please check your email.";
                        return RedirectToAction(nameof(VerifyEmail));
                    }

                    TempData["Success"] = "Registration successful! You can now sign in.";
                    return RedirectToAction(nameof(Login));
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration exception for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during registration.");
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            var userId = TempData["UserId"];
            var email = TempData["Email"];

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            TempData.Keep("UserId");
            TempData.Keep("Email");

            var model = new VerifyEmailViewModel
            {
                UserId = userId is Guid guid ? guid : Guid.Parse(userId.ToString()!),
                Email = email?.ToString()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _authService.ConfirmEmailAsync(model);

                if (result.IsSuccess)
                {
                    TempData["Success"] = "Email verified successfully! You can now log in.";
                    return RedirectToAction(nameof(Login));
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Verification failed.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email verification failed for user: {UserId}", model.UserId);
                ModelState.AddModelError(string.Empty, "An error occurred during verification.");
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(string email)
        {
            try
            {
                var result = await _authService.ResendOtpAsync(email);

                if (result.IsSuccess)
                {
                    TempData["Success"] = "Verification code sent successfully.";
                }
                else
                {
                    TempData["Error"] = result.Message ?? "Failed to resend code.";
                }

                return RedirectToAction(nameof(VerifyEmail), new { email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend OTP failed for email: {Email}", email);
                TempData["Error"] = "An error occurred.";
                return RedirectToAction(nameof(VerifyEmail), new { email });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult VerifyTwoFactor()
        {
            var userId = TempData["UserId"];

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            TempData.Keep("UserId");
            TempData.Keep("TwoFactorToken");

            return View(new VerifyTwoFactorViewModel
            {
                UserId = userId is Guid guid ? guid : Guid.Parse(userId.ToString()!)
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _authService.VerifyOtpAsync(model);

                if (result.IsSuccess)
                {
                    TempData["Success"] = "Login successful!";
                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Invalid code.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "2FA verification failed for user: {UserId}", model.UserId);
                ModelState.AddModelError(string.Empty, "An error occurred.");
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authService.LogoutAsync();
                TempData["Success"] = "You have been logged out successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
            }

            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _authService.RequestPasswordResetAsync(model);

                TempData["Success"] = "If your email exists, you will receive a password reset link.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password reset request failed for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred.");
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid password reset link.";
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _authService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                {
                    TempData["Success"] = "Password reset successful! You can now log in.";
                    return RedirectToAction(nameof(Login));
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Password reset failed.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password reset failed for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred.");
                return View(model);
            }
        }
    }
}