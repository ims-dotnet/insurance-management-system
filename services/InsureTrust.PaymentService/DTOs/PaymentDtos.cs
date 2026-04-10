using System.ComponentModel.DataAnnotations;

namespace InsureTrust.PaymentService.DTOs
{
    public class InitiatePaymentDto
    {
        [Required]
        public int UserPolicyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
    }

    // Response DTOs
    public class PaymentDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public int UserPolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }

    public class InitiatePaymentResponseDto
    {
        public string PaymentNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty; // For future gateway integration
    }

}
