using System.ComponentModel.DataAnnotations;

namespace InsureTrust.Web.Models
{
    public class PolicyPaymentViewModel
    {
        // UserPolicyId is assigned by the payment service on success (not user-entered)
        public int? UserPolicyId { get; set; }
        public string? PolicyNumber { get; set; }

        [Required(ErrorMessage = "Please select a payment category.")]
        public string PaymentCategory { get; set; } = "UPI";

        [Required(ErrorMessage = "Please select a payment option.")]
        public string PaymentMethod { get; set; } = "GPay";

        public bool IsRenewal { get; set; } = false;
        public int PolicyId { get; set; }

        public string? PaymentNumber { get; set; }
        public decimal Amount { get; set; }
        public int Tenure { get; set; }
        public string? Status { get; set; }
        public string? TransactionId { get; set; }
        public string? RedirectUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }

        // Demo verification fields
        public string? UpiId { get; set; }
        public string? UpiApp { get; set; }

        public string? CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryMonthYear { get; set; }
        public string? Cvv { get; set; }

        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
    }
}