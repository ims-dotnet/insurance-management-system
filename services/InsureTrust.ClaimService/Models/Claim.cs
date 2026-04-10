using System.ComponentModel.DataAnnotations;

namespace InsureTrust.ClaimService.Models;

public class Claim
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string ClaimNumber { get; set; } = string.Empty;  // CLM4001

    [Required]
    public int UserPolicyId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ClaimStatus { get; set; } = "Pending";  // Pending, Approved, Denied

    [Precision(18, 2)]
    public decimal MaturityAmount { get; set; }

    [MaxLength(500)]
    public string? AdminRemarks { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }

    public string? DocumentPathsJson { get; set; }  // JSON array of document paths
}

