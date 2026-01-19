namespace HMS.Authentication.Infrastructure.Configuration
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string InstanceName { get; set; } = "HMS_Auth_";
        public int DefaultCacheExpirationMinutes { get; set; } = 30;
    }
}
