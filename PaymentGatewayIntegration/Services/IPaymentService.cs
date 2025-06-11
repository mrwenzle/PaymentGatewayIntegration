// IPaymentService.cs
using Newtonsoft.Json;
using PaymentGatewayIntegration.Configuration;
using PaymentGatewayIntegration.Logging;
using PaymentGatewayIntegration.Models;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Text;

namespace PaymentGatewayIntegration.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    }
}


