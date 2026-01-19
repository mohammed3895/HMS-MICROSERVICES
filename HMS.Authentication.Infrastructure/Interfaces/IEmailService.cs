using HMS.Authentication.Domain.Entities;

namespace HMS.Authentication.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendEmailConfirmationAsync(string email, string firstName, string confirmationLink);
        Task SendWelcomeEmailAsync(ApplicationUser user);
        Task SendPasswordResetEmailAsync(ApplicationUser user, string resetToken);
        Task SendPasswordChangedEmailAsync(ApplicationUser user);
        Task Send2FAEnabledEmailAsync(ApplicationUser user);
        Task SendRecoveryCodeUsedEmailAsync(ApplicationUser user);
        Task SendNewDeviceLoginEmailAsync(ApplicationUser user, string deviceInfo);
        Task SendAccountLockedEmailAsync(ApplicationUser user);
        Task SendOtpEmailAsync(ApplicationUser user, string otpCode);
    }
}
