using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// PaymentMethod.cs
namespace PaymentGatewayIntegration.Models
{
    public enum PaymentMethod
    {
        CreditCard,
        BankSlip,
        Pix
    }
}

// CreditCardInfo.cs
namespace PaymentGatewayIntegration.Models
{
    public class CreditCardInfo
    {
        public string CardNumber { get; set; }
        public string HolderName { get; set; }
        public string ExpirationDate { get; set; } // MM/YY
        public string Cvv { get; set; }

        public bool Validate()
        {
            // Validação básica
            if (string.IsNullOrWhiteSpace(CardNumber) || CardNumber.Length < 13 || CardNumber.Length > 19)
                return false;

            if (string.IsNullOrWhiteSpace(HolderName))
                return false;

            if (string.IsNullOrWhiteSpace(ExpirationDate) || !ExpirationDate.Contains('/'))
                return false;

            if (string.IsNullOrWhiteSpace(Cvv) || Cvv.Length < 3 || Cvv.Length > 4)
                return false;

            // Validação de data de expiração
            var parts = ExpirationDate.Split('/');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out int month) || !int.TryParse(parts[1], out int year))
                return false;

            if (month < 1 || month > 12)
                return false;

            // Converter para ano de 4 dígitos
            year += 2000;

            // Verificar se o cartão não está expirado
            var today = DateTime.Today;
            var expirationDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

            return expirationDate > today;
        }
    }
}

// PaymentRequest.cs
namespace PaymentGatewayIntegration.Models
{
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BRL";
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerDocument { get; set; } // CPF/CNPJ
        public PaymentMethod PaymentMethod { get; set; }
        public CreditCardInfo CreditCardInfo { get; set; }
        public string PixKey { get; set; }
        public string CallbackUrl { get; set; }
        public string Description { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(OrderId))
                return false;

            if (Amount <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerEmail))
                return false;

            // Validação específica por método de pagamento
            switch (PaymentMethod)
            {
                case PaymentMethod.CreditCard:
                    return CreditCardInfo != null && CreditCardInfo.Validate();

                case PaymentMethod.Pix:
                    return !string.IsNullOrWhiteSpace(PixKey);

                case PaymentMethod.BankSlip:
                    return !string.IsNullOrWhiteSpace(CustomerDocument);

                default:
                    return false;
            }
        }
    }
}

// PaymentResponse.cs
namespace PaymentGatewayIntegration.Models
{
    public class PaymentResponse
    {
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string PaymentUrl { get; set; } // URL para boleto ou QR code PIX
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsSuccess => Status == "approved" || Status == "pending";
    }
}