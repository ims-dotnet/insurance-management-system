namespace InsureTrust.ClaimService.Models
{
    public class Claim
    {
        public int Id { get; set; }

        public string ClaimNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int UserPolicyId { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal MaturityAmount { get; set; }

        public string ClaimStatus { get; set; } = "Pending";

        public string? AdminRemarks { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        public string? DocumentPathsJson { get; set; }
    }
}