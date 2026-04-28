namespace InsureTrust.PaymentService.DTOs
{
    public class InitiateRenewalPaymentDto
    {
        public int UserPolicyId { get; set; }

       
        public string PaymentMethod { get; set; } = string.Empty;
    }
}