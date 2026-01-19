using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Configuration;
using HMS.Authentication.Infrastructure.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HMS.Authentication.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail} with subject: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailConfirmationAsync(string email, string firstName, string confirmationLink)
        {
            var subject = "Confirm Your Email Address";
            var body = GetEmailTemplate("EmailConfirmation")
                .Replace("{{FirstName}}", firstName)
                .Replace("{{ConfirmationLink}}", confirmationLink)
                .Replace("{{ExpiryTime}}", "24 hours");

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(ApplicationUser user)
        {
            var subject = "Welcome to Hospital Management System";
            var body = GetEmailTemplate("Welcome")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{Email}}", user.Email!);

            await SendEmailAsync(user.Email!, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(ApplicationUser user, string resetToken)
        {
            var resetUrl = $"{_emailSettings.WebAppUrl}/reset-password?token={resetToken}&email={user.Email}";
            var subject = "Reset Your Password";
            var body = GetEmailTemplate("PasswordReset")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{ResetUrl}}", resetUrl)
                .Replace("{{ExpiryTime}}", "1 hour");

            await SendEmailAsync(user.Email!, subject, body);
        }

        public async Task SendPasswordChangedEmailAsync(ApplicationUser user)
        {
            var subject = "Password Changed Successfully";
            var body = GetEmailTemplate("PasswordChanged")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{DateTime}}", DateTime.UtcNow.ToString("f"));

            await SendEmailAsync(user.Email!, subject, body);
        }

        public async Task Send2FAEnabledEmailAsync(ApplicationUser user)
        {
            var subject = "Two-Factor Authentication Enabled";
            var body = GetEmailTemplate("2FAEnabled")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{DateTime}}", DateTime.UtcNow.ToString("f"));

            await SendEmailAsync(user.Email!, subject, body);
        }

        public async Task SendRecoveryCodeUsedEmailAsync(ApplicationUser user)
        {
            var subject = "Recovery Code Used";
            var body = GetEmailTemplate("RecoveryCodeUsed")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{DateTime}}", DateTime.UtcNow.ToString("f"));

            await SendEmailAsync(user.Email!, subject, body);
        }

        public async Task SendNewDeviceLoginEmailAsync(ApplicationUser user, string deviceInfo)
        {
            var subject = "New Device Login Detected";
            var body = GetEmailTemplate("NewDeviceLogin")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{DeviceInfo}}", deviceInfo)
                .Replace("{{DateTime}}", DateTime.UtcNow.ToString("f"));

            await SendEmailAsync(user.Email!, subject, body);
        }

        public async Task SendAccountLockedEmailAsync(ApplicationUser user)
        {
            var subject = "Account Locked";
            var body = GetEmailTemplate("AccountLocked")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{DateTime}}", DateTime.UtcNow.ToString("f"));

            await SendEmailAsync(user.Email!, subject, body);
        }

        public async Task SendOtpEmailAsync(ApplicationUser user, string otpCode)
        {
            var subject = "Your One-Time Password";
            var body = GetEmailTemplate("OTP")
                .Replace("{{UserName}}", $"{user.FirstName} {user.LastName}")
                .Replace("{{OtpCode}}", otpCode)
                .Replace("{{ExpiryTime}}", "10 minutes");

            await SendEmailAsync(user.Email!, subject, body);
        }

        private string GetEmailTemplate(string templateName)
        {
            return templateName switch
            {
                "EmailConfirmation" => EmailConfirmationTemplate,
                "Welcome" => WelcomeEmailTemplate,
                "PasswordReset" => PasswordResetTemplate,
                "PasswordChanged" => PasswordChangedTemplate,
                "2FAEnabled" => TwoFactorEnabledTemplate,
                "RecoveryCodeUsed" => RecoveryCodeUsedTemplate,
                "NewDeviceLogin" => NewDeviceLoginTemplate,
                "AccountLocked" => AccountLockedTemplate,
                "OTP" => OtpTemplate,
                _ => throw new ArgumentException("Invalid template name")
            };
        }

        #region Email Templates

        private const string EmailConfirmationTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Confirm Your Email</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>Hospital Management System</h1>
                            <p style='color: #ffffff; margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>Email Confirmation</p>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Hello, {{FirstName}}</h2>
                            <p style='color: #666666; line-height: 1.6; margin: 0 0 15px 0; font-size: 16px;'>
                                Thank you for registering with our Hospital Management System. Please confirm your email address by clicking the button below:
                            </p>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td align='center' style='padding: 20px 0;'>
                                        <a href='{{ConfirmationLink}}' style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; padding: 15px 40px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Confirm Email</a>
                                    </td>
                                </tr>
                            </table>
                            <p style='color: #666666; line-height: 1.6; margin: 20px 0 0 0; font-size: 14px;'>
                                This confirmation link will expire in {{ExpiryTime}}. If you didn't register for this account, please ignore this email.
                            </p>
                            <div style='background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 15px; margin: 20px 0;'>
                                <p style='color: #666666; margin: 0; font-size: 14px;'>
                                    <strong>Tip:</strong> If the button above doesn't work, you can also copy and paste the link below in your browser:
                                </p>
                                <p style='color: #667eea; margin: 10px 0 0 0; font-size: 12px; word-break: break-all;'>
                                    {{ConfirmationLink}}
                                </p>
                            </div>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 30px; text-align: center; border-top: 1px solid #e9ecef;'>
                            <p style='color: #999999; margin: 0 0 10px 0; font-size: 14px;'>
                                © 2025 Hospital Management System. All rights reserved.
                            </p>
                            <p style='color: #999999; margin: 0; font-size: 12px;'>
                                This is an automated message. Please do not reply to this email.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string WelcomeEmailTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to HMS</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>Hospital Management System</h1>
                            <p style='color: #ffffff; margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>Professional Healthcare Solutions</p>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Welcome, {{UserName}}!</h2>
                            <p style='color: #666666; line-height: 1.6; margin: 0 0 15px 0; font-size: 16px;'>
                                Thank you for joining our Hospital Management System. Your account has been successfully created.
                            </p>
                            <p style='color: #666666; line-height: 1.6; margin: 0 0 15px 0; font-size: 16px;'>
                                <strong>Email:</strong> {{Email}}
                            </p>
                            <div style='background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 15px; margin: 20px 0;'>
                                <p style='color: #666666; margin: 0; font-size: 14px;'>
                                    <strong>Security Tip:</strong> Keep your login credentials secure and enable two-factor authentication for enhanced security.
                                </p>
                            </div>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 30px; text-align: center; border-top: 1px solid #e9ecef;'>
                            <p style='color: #999999; margin: 0 0 10px 0; font-size: 14px;'>
                                © 2025 Hospital Management System. All rights reserved.
                            </p>
                            <p style='color: #999999; margin: 0; font-size: 12px;'>
                                This is an automated message. Please do not reply to this email.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string PasswordResetTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>Password Reset Request</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Hello, {{UserName}}</h2>
                            <p style='color: #666666; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                We received a request to reset your password. Click the button below to proceed:
                            </p>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td align='center' style='padding: 20px 0;'>
                                        <a href='{{ResetUrl}}' style='background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: #ffffff; padding: 15px 40px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Reset Password</a>
                                    </td>
                                </tr>
                            </table>
                            <p style='color: #666666; line-height: 1.6; margin: 20px 0 0 0; font-size: 14px;'>
                                This link will expire in {{ExpiryTime}}. If you didn't request this, please ignore this email.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 30px; text-align: center;'>
                            <p style='color: #999999; margin: 0; font-size: 14px;'>
                                © 2025 Hospital Management System
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string PasswordChangedTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0;'>Password Changed</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0;'>Hello, {{UserName}}</h2>
                            <p style='color: #666666; line-height: 1.6; margin: 0 0 15px 0;'>
                                Your password was successfully changed on {{DateTime}}.
                            </p>
                            <div style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0;'>
                                <p style='color: #856404; margin: 0;'>
                                    If you didn't make this change, please contact support immediately.
                                </p>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string TwoFactorEnabledTemplate = @"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px;'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0;'>2FA Enabled</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333;'>Hello, {{UserName}}</h2>
                            <p style='color: #666666; line-height: 1.6;'>
                                Two-factor authentication has been successfully enabled on your account at {{DateTime}}.
                            </p>
                            <p style='color: #666666;'>
                                Your account is now more secure. You'll need to enter a code from your authenticator app each time you log in.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string RecoveryCodeUsedTemplate = @"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px;'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0;'>Recovery Code Used</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333;'>Hello, {{UserName}}</h2>
                            <p style='color: #666666;'>
                                A recovery code was used to access your account at {{DateTime}}.
                            </p>
                            <div style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0;'>
                                <p style='color: #856404; margin: 0;'>
                                    If this wasn't you, please secure your account immediately.
                                </p>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string NewDeviceLoginTemplate = @"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px;'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #fa709a 0%, #fee140 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0;'>New Device Login</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333;'>Hello, {{UserName}}</h2>
                            <p style='color: #666666;'>
                                A new device logged into your account at {{DateTime}}.
                            </p>
                            <p style='color: #666666;'><strong>Device:</strong> {{DeviceInfo}}</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string AccountLockedTemplate = @"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px;'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #eb3349 0%, #f45c43 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0;'>Account Locked</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='color: #333333;'>Hello, {{UserName}}</h2>
                            <p style='color: #666666;'>
                                Your account was locked at {{DateTime}} due to multiple failed login attempts.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        private const string OtpTemplate = @"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f7fa; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px;'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0;'>Your OTP Code</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px; text-align: center;'>
                            <h2 style='color: #333333;'>Hello, {{UserName}}</h2>
                            <p style='color: #666666;'>Your one-time password is:</p>
                            <div style='background-color: #f8f9fa; border: 2px dashed #667eea; padding: 20px; margin: 20px 0; font-size: 32px; font-weight: bold; color: #667eea; letter-spacing: 5px;'>
                                {{OtpCode}}
                            </div>
                            <p style='color: #666666; font-size: 14px;'>This code will expire in {{ExpiryTime}}.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        #endregion
    }
}