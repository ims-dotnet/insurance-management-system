namespace InsureTrust.PaymentService.Helpers
{
    public static class PaymentHelper
    {
        public static string GeneratePaymentNumber()
        {
            return $"PAY{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        public static int GenerateUserPolicyId()
        {
            return int.Parse(DateTime.UtcNow.ToString("ddHHmmss"));
        }
    }
}
