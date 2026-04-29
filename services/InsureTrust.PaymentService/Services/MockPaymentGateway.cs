namespace InsureTrust.PaymentService.Services
{
    public class MockPaymentGateway :IPaymentGateway
    {
        public Task<PaymentResult>ProcessAsync(decimal amount,string paymentMethod)
        {
            return Task.FromResult(new PaymentResult
            { Success=true,
            TransactionId=Guid.NewGuid().ToString()
            });

        }
    }
}
