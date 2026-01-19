namespace HMS.Web.Constants
{
    public static class AppConstants
    {
        public const string DefaultTimezone = "America/New_York";
        public const int SessionTimeoutMinutes = 30;
        public const int MaxUploadSizeMB = 10;

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Doctor = "Doctor";
            public const string Patient = "Patient";
            public const string Staff = "Staff";
        }

        public static class AppointmentStatus
        {
            public const string Scheduled = "Scheduled";
            public const string Pending = "Pending";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
            public const string NoShow = "NoShow";
        }

        public static class Messages
        {
            public const string LoginSuccess = "Login successful! Welcome back.";
            public const string LoginFailed = "Invalid email or password.";
            public const string RegistrationSuccess = "Registration successful! Please log in.";
            public const string RegistrationFailed = "Registration failed. Please try again.";
            public const string AppointmentCreated = "Appointment created successfully!";
            public const string AppointmentUpdated = "Appointment updated successfully!";
            public const string AppointmentCancelled = "Appointment cancelled successfully!";
            public const string ServiceError = "An error occurred while processing your request.";
            public const string Unauthorized = "You do not have permission to access this resource.";
        }
    }
}
