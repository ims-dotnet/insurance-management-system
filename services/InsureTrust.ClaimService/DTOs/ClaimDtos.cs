using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InsureTrust.ClaimService.DTOs
{
    public class SubmitClaimDto
    {
        public string Description { get; set; } = string.Empty;
        
        public decimal MaturityAmount { get; set; }
        
        public List<IFormFile> Documents { get; set; } = new();
    }

    public class UpdateClaimDto
    {
        public string Action { get; set; } = string.Empty;  // Approve, Deny
        
        public string? AdminRemarks { get; set; }
    }

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