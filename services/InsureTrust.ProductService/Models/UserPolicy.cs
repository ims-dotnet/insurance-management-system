using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureTrust.ProductService.Models;

public class UserPolicy
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string PolicyNumber { get; set; } = string.Empty;  // POL2001

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PolicyTypeId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";  // Pending, Active, Rejected, Expired

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public DateTime ExpiryDate { get; set; }

    public int Tenure { get; set; }  // in months

    [Precision(18, 2)]
    public decimal PackageAmount { get; set; }  // 5000, 10000, 15000

    public string? DynamicFieldsJson { get; set; }  // JSON of dynamic form data

    [MaxLength(500)]
    public string? AdminRemarks { get; set; }

    // Navigation Properties
    [ForeignKey("PolicyTypeId")]
    public PolicyType PolicyType { get; set; } = null!;
}

