using Microsoft.AspNetCore.Http;

namespace InsureTrust.QueryService.DTOs
{
    public class CreateSupportQueryDto
    {
        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public IFormFile? Attachment { get; set; }
    }

    public class UpdateSupportStatusDto
    {
        public string Status { get; set; } = string.Empty;

        public string? AdminResponse { get; set; }
    }

    public class SupportQueryDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}