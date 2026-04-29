namespace InsureTrust.PaymentService.DTOs
{
    public class InitiateFirstPaymentDto
    {
       
        public int PolicyId { get; set; }

        
        public string PaymentMethod { get; set; } = string.Empty;
    }
}