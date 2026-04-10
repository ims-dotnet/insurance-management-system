using System.ComponentModel.DataAnnotations;

namespace InsureTrust.ClaimService.DTOs
{
    public class CreateSupportQueryDto
    {
        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public IFormFile? Attachment { get; set; }
    }

    public class UpdateSupportStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;  // Pending, InProgress, Resolved

        [MaxLength(1000)]
        public string? AdminResponse { get; set; }
    }

    // Response DTOs
    public class SupportQueryDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

}
