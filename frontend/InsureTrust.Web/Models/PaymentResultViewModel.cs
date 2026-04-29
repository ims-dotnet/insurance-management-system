namespace InsureTrust.Web.Models
{
    public class PaymentResultViewModel
    {
        public int ?UserPolicyId { get; set; }
        public string? PolicyNumber { get; set; }
        public int? GeneratedUserPolicyId { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public bool IsRenewal { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
