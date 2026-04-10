using System.ComponentModel.DataAnnotations;

namespace InsureTrust.ProductService.Models;

public class PolicyType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;  // Term Life, Health, etc.

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;  // Personal, Business

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Icon { get; set; } = string.Empty;  // FontAwesome class

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<PolicyTerm> Terms { get; set; } = new List<PolicyTerm>();
    public ICollection<PolicyRequiredField> RequiredFields { get; set; } = new List<PolicyRequiredField>();
    public ICollection<UserPolicy> UserPolicies { get; set; } = new List<UserPolicy>();
}

