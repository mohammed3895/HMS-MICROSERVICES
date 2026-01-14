namespace HMS.Authentication.Domain.Enums
{
    public enum AuditAction
    {
        // User Management
        UserCreated = 1,
        UserUpdated = 2,
        UserDeleted = 3,
        UserActivated = 4,
        UserDeactivated = 5,
        UserPasswordChanged = 6,
        UserPasswordReset = 7,

        // Authentication
        Login = 10,
        Logout = 11,
        LoginFailed = 12,
        TwoFactorEnabled = 13,
        TwoFactorDisabled = 14,

        // Role Management
        RoleCreated = 20,
        RoleUpdated = 21,
        RoleDeleted = 22,
        RoleAssigned = 23,
        RoleUnassigned = 24,

        // Permission Management
        PermissionGranted = 30,
        PermissionRevoked = 31,

        // Profile Management
        ProfileUpdated = 40,
        ProfilePictureChanged = 41,

        // Security
        OtpGenerated = 50,
        OtpVerified = 51,
        OtpExpired = 52,
        SecurityQuestionUpdated = 53,

        // Data Access
        DataViewed = 60,
        DataExported = 61,
        DataImported = 62,

        // System
        SystemConfigChanged = 70,
        SystemMaintenance = 71,

        // General CRUD
        Created = 100,
        Read = 101,
        Updated = 102,
        Deleted = 103,

        // Other
        Other = 999
    }
}