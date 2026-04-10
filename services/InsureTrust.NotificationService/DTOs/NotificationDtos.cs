using System.ComponentModel.DataAnnotations;

namespace InsureTrust.NotificationService.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SendNotificationDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public string ColorCode { get; set; } = string.Empty;

    public string Feature { get; set; } = string.Empty;
}

public class UnreadCountResponseDto
{
    public int Count { get; set; }
}

