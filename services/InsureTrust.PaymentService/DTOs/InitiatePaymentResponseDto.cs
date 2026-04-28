namespace InsureTrust.PaymentService.DTOs
{
    public class InitiatePaymentResponseDto
    {
        public string PaymentNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int? GeneratedUserPolicyId { get; set; }
    }
}