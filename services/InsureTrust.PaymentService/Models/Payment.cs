using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.PaymentService.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

       
        public string PaymentNumber { get; set; } = string.Empty;

        
        public int UserId { get; set; }

        // First purchase ke liye
        public int? PolicyId { get; set; }

        // Renewal ke liye
        public int? UserPolicyId { get; set; }

        public decimal Amount { get; set; }

       
        public string Status { get; set; } = "Pending";

        public string PaymentMethod { get; set; } = string.Empty;

        
        public string TransactionId { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        
        public string? Remarks { get; set; }
    }
}