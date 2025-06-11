// PayGatewayService.cs
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using PaymentGatewayIntegration.Configuration;
using PaymentGatewayIntegration.Logging;
using PaymentGatewayIntegration.Models;
using Polly;
using Polly.Retry;

namespace PaymentGatewayIntegration.Services
{
    public class PayGatewayService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PayGatewayConfig _config;
        private readonly FileLogger _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public PayGatewayService(PayGatewayConfig config, FileLogger logger)
        {
            _config = config;
            _logger = logger;

            // Configurar HttpClient
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_config.ApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
            };

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config.ApiKey);

            // Configurar política de retry
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    _config.MaxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Tentativa {retryCount} falhou. Aguardando {timeSpan.TotalSeconds} segundos antes de tentar novamente.");
                    });
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                // Validar a requisição
                if (!request.Validate())
                {
                    _logger.LogError($"Requisição de pagamento inválida para o pedido {request.OrderId}");
                    return new PaymentResponse
                    {
                        OrderId = request.OrderId,
                        Status = "error",
                        ErrorCode = "INVALID_REQUEST",
                        ErrorMessage = "Dados de pagamento inválidos"
                    };
                }

                _logger.LogInfo($"Iniciando processamento de pagamento para o pedido {request.OrderId}. Método: {request.PaymentMethod}");

                // Serializar a requisição
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Enviar requisição com retry policy
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInfo($"Enviando requisição para API de pagamento. Pedido: {request.OrderId}");
                    return await _httpClient.PostAsync("/api/payments", content);
                });

                // Processar resposta
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInfo($"Resposta recebida para o pedido {request.OrderId}. Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(responseContent);
                    _logger.LogInfo($"Pagamento processado. TransactionId: {paymentResponse.TransactionId}, Status: {paymentResponse.Status}");
                    return paymentResponse;
                }
                else
                {
                    _logger.LogError($"Erro ao processar pagamento. Status: {response.StatusCode}, Resposta: {responseContent}");
                    return new PaymentResponse
                    {
                        OrderId = request.OrderId,
                        Status = "error",
                        ErrorCode = $"HTTP_{(int)response.StatusCode}",
                        ErrorMessage = $"Erro na comunicação com a API: {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exceção ao processar pagamento para o pedido {request.OrderId}", ex);
                return new PaymentResponse
                {
                    OrderId = request.OrderId,
                    Status = "error",
                    ErrorCode = "EXCEPTION",
                    ErrorMessage = $"Erro interno: {ex.Message}"
                };
            }
        }
    }
}