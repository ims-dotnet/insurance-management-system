using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InsureTrust.Web.Models
{
    public class SubmitClaimViewModel
    {
        [Required]
        public int PolicyId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal MaturityAmount { get; set; }
        
        public List<IFormFile> Documents { get; set; } = new();
    }

    public class ClaimViewModel
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ClaimStatus { get; set; } = string.Empty;
        public decimal MaturityAmount { get; set; }
        public string? AdminRemarks { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<string> DocumentUrls { get; set; } = new();
    }
}
