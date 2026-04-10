using System.ComponentModel.DataAnnotations;

namespace InsureTrust.QueryService.Models;

public class SupportQuery
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string TicketNumber { get; set; } = string.Empty;  // SUP3001

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";  // Pending, InProgress, Resolved

    [MaxLength(500)]
    public string? AttachmentPath { get; set; }

    [MaxLength(1000)]
    public string? AdminResponse { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}

