using System.ComponentModel.DataAnnotations;

namespace InsureTrust.ClaimService.DTOs
{
    public class SubmitClaimDto
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal MaturityAmount { get; set; }

        public List<IFormFile> Documents { get; set; } = new();  // Up to 12 files
    }

    public class UpdateClaimDto
    {
        [Required]
        public string Action { get; set; } = string.Empty;  // Approve, Deny

        [MaxLength(500)]
        public string? AdminRemarks { get; set; }
    }

    // Response DTOs
    public class ClaimDto
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public string PolicyTypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ClaimStatus { get; set; } = string.Empty;
        public decimal MaturityAmount { get; set; }
        public string? AdminRemarks { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<string> DocumentUrls { get; set; } = new();
    }

}
