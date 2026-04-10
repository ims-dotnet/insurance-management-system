using System.ComponentModel.DataAnnotations;

namespace InsureTrust.PaymentService.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string PaymentNumber { get; set; } = string.Empty;  // PAY5001

    [Required]
    public int UserId { get; set; }

    [Required]
    public int UserPolicyId { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";  // Pending, Success, Failed

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;  // Card, UPI, NetBanking

    [MaxLength(100)]
    public string TransactionId { get; set; } = string.Empty;  // From gateway

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Remarks { get; set; }
}

