namespace InsureTrust.Web.Models
{
    public class RenewalPaymentViewModel
    {
        public int UserPolicyId { get; set; }

        public string PaymentMethod { get; set; } = "UPI";

        public bool IsRenewal { get; set; } = true;

        public string? PaymentNumber { get; set; }

        public decimal Amount { get; set; }

        public string? Status { get; set; }

        public string? TransactionId { get; set; }

        public string? RedirectUrl { get; set; }

        public string? ErrorMessage { get; set; }

        public bool IsSuccess { get; set; }
    }
}