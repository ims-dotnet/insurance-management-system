using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureTrust.ProductService.Models;

public class PolicyTerm
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PolicyTypeId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string TermText { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;

    // Navigation Property
    [ForeignKey("PolicyTypeId")]
    public PolicyType PolicyType { get; set; } = null!;
}

