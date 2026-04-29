namespace InsureTrust.PaymentService.DTOs
{
    public class InitiateRenewalPaymentDto
    {
        public string PolicyNumber { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;
    }
}