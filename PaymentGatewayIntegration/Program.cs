using PaymentGatewayIntegration.Configuration;
using PaymentGatewayIntegration.Logging;
using PaymentGatewayIntegration.Models;
using PaymentGatewayIntegration.Services;

namespace PaymentGatewayIntegration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("PayGateway - Teste de Integração");
            Console.WriteLine("================================");

            // Configuração
            var config = new PayGatewayConfig
            {
                ApiBaseUrl = "https://api.paygateway.example.com", // URL fictícia
                ApiKey = "seu_api_key_aqui",
                TimeoutSeconds = 30,
                MaxRetries = 3,
                LogFilePath = Path.Combine(Environment.CurrentDirectory, "logs", "payment_gateway.log")
            };

            // Inicializar logger
            var logger = new FileLogger(config.LogFilePath);
            logger.LogInfo("Aplicação iniciada");

            // Inicializar serviço de pagamento
            var paymentService = new PayGatewayService(config, logger);

            try
            {
                // Exemplo 1: Pagamento com cartão de crédito
                Console.WriteLine("\nProcessando pagamento com cartão de crédito...");
                var creditCardRequest = new PaymentRequest
                {
                    OrderId = $"ORDER-{DateTime.Now:yyyyMMddHHmmss}",
                    Amount = 150.75m,
                    CustomerName = "João Silva",
                    CustomerEmail = "joao@example.com",
                    CustomerDocument = "123.456.789-00",
                    PaymentMethod = PaymentMethod.CreditCard,
                    Description = "Compra de roupas - ModaStyle",
                    CallbackUrl = "https://modastyle.example.com/payment/callback",
                    CreditCardInfo = new CreditCardInfo
                    {
                        CardNumber = "4111111111111111", // Número de teste
                        HolderName = "JOAO SILVA",
                        ExpirationDate = "12/25",
                        Cvv = "123"
                    }
                };

                // Em um cenário real, você se conectaria à API real
                // Como estamos simulando, vamos apenas mostrar o que seria enviado
                Console.WriteLine("Dados que seriam enviados à API:");
                Console.WriteLine($"- Pedido: {creditCardRequest.OrderId}");
                Console.WriteLine($"- Valor: R$ {creditCardRequest.Amount}");
                Console.WriteLine($"- Cliente: {creditCardRequest.CustomerName}");
                Console.WriteLine($"- Método: Cartão de Crédito");
                Console.WriteLine($"- Cartão: **** **** **** {creditCardRequest.CreditCardInfo.CardNumber.Substring(12)}");

                // Exemplo 2: Pagamento com PIX
                Console.WriteLine("\nProcessando pagamento com PIX...");
                var pixRequest = new PaymentRequest
                {
                    OrderId = $"ORDER-{DateTime.Now:yyyyMMddHHmmss}",
                    Amount = 89.90m,
                    CustomerName = "Maria Souza",
                    CustomerEmail = "maria@example.com",
                    CustomerDocument = "987.654.321-00",
                    PaymentMethod = PaymentMethod.Pix,
                    Description = "Compra de roupas - ModaStyle",
                    CallbackUrl = "https://modastyle.example.com/payment/callback",
                    PixKey = "maria@example.com"
                };

                Console.WriteLine("Dados que seriam enviados à API:");
                Console.WriteLine($"- Pedido: {pixRequest.OrderId}");
                Console.WriteLine($"- Valor: R$ {pixRequest.Amount}");
                Console.WriteLine($"- Cliente: {pixRequest.CustomerName}");
                Console.WriteLine($"- Método: PIX");
                Console.WriteLine($"- Chave PIX: {pixRequest.PixKey}");

                // Exemplo 3: Pagamento com boleto
                Console.WriteLine("\nProcessando pagamento com boleto...");
                var boletoRequest = new PaymentRequest
                {
                    OrderId = $"ORDER-{DateTime.Now:yyyyMMddHHmmss}",
                    Amount = 299.99m,
                    CustomerName = "Pedro Santos",
                    CustomerEmail = "pedro@example.com",
                    CustomerDocument = "111.222.333-44",
                    PaymentMethod = PaymentMethod.BankSlip,
                    Description = "Compra de roupas - ModaStyle",
                    CallbackUrl = "https://modastyle.example.com/payment/callback"
                };

                Console.WriteLine("Dados que seriam enviados à API:");
                Console.WriteLine($"- Pedido: {boletoRequest.OrderId}");
                Console.WriteLine($"- Valor: R$ {boletoRequest.Amount}");
                Console.WriteLine($"- Cliente: {boletoRequest.CustomerName}");
                Console.WriteLine($"- Método: Boleto Bancário");
                Console.WriteLine($"- CPF: {boletoRequest.CustomerDocument}");

                // Em um ambiente real, você chamaria:
                // var response = await paymentService.ProcessPaymentAsync(creditCardRequest);
                // E processaria a resposta

                Console.WriteLine("\nSimulação concluída. Verifique o arquivo de log para mais detalhes.");
                logger.LogInfo("Simulação de pagamentos concluída com sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro durante a execução: {ex.Message}");
                logger.LogError("Erro durante a execução da aplicação", ex);
            }

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}