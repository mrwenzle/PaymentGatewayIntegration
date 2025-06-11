// PayGatewayConfig.cs
namespace PaymentGatewayIntegration.Configuration
{
    public class PayGatewayConfig
    {
        public string ApiBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public int MaxRetries { get; set; } = 3;
        public string LogFilePath { get; set; }
    }
}