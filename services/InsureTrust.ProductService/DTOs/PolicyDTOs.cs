using System.ComponentModel.DataAnnotations;

namespace InsureTrust.ProductService.DTOs
{
    public class CreatePolicyTypeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Icon { get; set; } = string.Empty;
    }

    public class CreatePolicyDto
    {
        [Required]
        public int PolicyTypeId { get; set; }

        [Required]
        [Range(12, 36)]
        public int Tenure { get; set; }  // months

        [Required]
        [Range(5000, 15000)]
        public decimal PackageAmount { get; set; }

        public Dictionary<string, string> DynamicFields { get; set; } = new();
    }

    public class ApprovePolicyDto
    {
        [Required]
        public string Action { get; set; } = string.Empty;  // Grant, Reject

        [MaxLength(500)]
        public string? AdminRemarks { get; set; }
    }

    public class EditPolicyDto
    {
        [Range(12, 36)]
        public int? Tenure { get; set; }

        [Range(5000, 15000)]
        public decimal? PackageAmount { get; set; }
    }

    public class PolicyTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<PolicyTermDto> Terms { get; set; } = new();
        public List<PolicyRequiredFieldDto> RequiredFields { get; set; } = new();
    }

    public class PolicyTermDto
    {
        public int Id { get; set; }
        public string TermText { get; set; } = string.Empty;
    }

    public class PolicyRequiredFieldDto
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public string? Placeholder { get; set; }
    }

    public class PolicyDto
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string PolicyTypeName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysLeft { get; set; }
        public decimal PackageAmount { get; set; }
        public int Tenure { get; set; }
        public string? AdminRemarks { get; set; }
    }

}
