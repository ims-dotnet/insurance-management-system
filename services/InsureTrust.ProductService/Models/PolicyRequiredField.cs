using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureTrust.ProductService.Models;


public class PolicyRequiredField
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PolicyTypeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FieldName { get; set; } = string.Empty;  // "KYC Document", "Nominee Name"

    [Required]
    [MaxLength(20)]
    public string FieldType { get; set; } = "text";  // text, date, file, number, email

    public bool IsMandatory { get; set; } = true;

    [MaxLength(200)]
    public string? Placeholder { get; set; }

    public int DisplayOrder { get; set; } = 0;

    // Navigation Property
    [ForeignKey("PolicyTypeId")]
    public PolicyType PolicyType { get; set; } = null!;
}

