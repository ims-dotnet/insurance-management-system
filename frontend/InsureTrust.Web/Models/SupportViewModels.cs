using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InsureTrust.Web.Models
{
    public class CreateSupportQueryViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public IFormFile? Attachment { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public SupportQueryViewModel? CreatedTicket { get; set; }
    }

    public class SupportQueryViewModel
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

    public class SupportQueryListViewModel
    {
        public List<SupportQueryViewModel> Queries { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class UpdateSupportStatusViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? AdminResponse { get; set; }
    }
}