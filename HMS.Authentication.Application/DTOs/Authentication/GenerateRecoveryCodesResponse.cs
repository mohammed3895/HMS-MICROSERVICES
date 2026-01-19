namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class GenerateRecoveryCodesResponse
    {
        public List<string> RecoveryCodes { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
