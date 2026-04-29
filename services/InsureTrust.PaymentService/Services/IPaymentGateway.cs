namespace InsureTrust.PaymentService.Services
{
    public interface IPaymentGateway
    {
        Task<PaymentResult> ProcessAsync(decimal amount, string paymentMethod);
    }
    public class PaymentResult {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    
    
    
    }

}

