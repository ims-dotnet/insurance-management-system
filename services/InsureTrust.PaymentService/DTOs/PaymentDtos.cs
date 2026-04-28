//using System.ComponentModel.DataAnnotations;

//namespace InsureTrust.PaymentService.DTOs
//{
//    //public class InitiatePaymentDto
//    {
//        // First purchase ke liye
//        public int? PolicyId { get; set; }

//        // Renewal ke liye
//        public int? UserPolicyId { get; set; }

//        [Required]
//        [MaxLength(50)]
//        public string PaymentMethod { get; set; } = string.Empty;

//        public bool IsRenewal { get; set; }

//        [Range(0.01, double.MaxValue)]
//        public decimal Amount { get; set; }
//    }

//    public class PaymentDto
//    {
//        public int Id { get; set; }
//        public string PaymentNumber { get; set; } = string.Empty;
//        public int? PolicyId { get; set; }
//        public int? UserPolicyId { get; set; }
//        public string PolicyNumber { get; set; } = string.Empty;
//        public decimal Amount { get; set; }
//        public string Status { get; set; } = string.Empty;
//        public string PaymentMethod { get; set; } = string.Empty;
//        public string TransactionId { get; set; } = string.Empty;
//        public DateTime PaymentDate { get; set; }
//        public string? Remarks { get; set; }
//    }

//    public class InitiatePaymentResponseDto
//    {
//        public string PaymentNumber { get; set; } = string.Empty;
//        public decimal Amount { get; set; }
//        public string Status { get; set; } = string.Empty;
//        public string TransactionId { get; set; } = string.Empty;
//        public string RedirectUrl { get; set; } = string.Empty;
//    }


//}

namespace InsureTrust.PaymentService.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public int? PolicyId { get; set; }
        public int? UserPolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public int UserId { get; set; }
        public string? Remarks { get; set; }
    }
}