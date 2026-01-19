namespace HMS.Authentication.Infrastructure.Configuration
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Hospital Management System";
        public bool EnableSsl { get; set; } = true;
        public string WebAppUrl { get; set; } = string.Empty;
    }
}
