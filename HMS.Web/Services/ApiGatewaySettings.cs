namespace HMS.Web.Services
{
    public class ApiGatewaySettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
    }
}
