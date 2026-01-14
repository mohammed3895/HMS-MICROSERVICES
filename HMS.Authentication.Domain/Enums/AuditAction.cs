namespace HMS.Authentication.Domain.Enums
{
    public enum AuditAction
    {
        Create = 1,
        Read = 2,
        Update = 3,
        Delete = 4,
        Login = 5,
        Logout = 6,
        FailedLogin = 7,
        PasswordChange = 8,
        RoleChange = 9,
        ProfileUpdate = 10
    }
}
