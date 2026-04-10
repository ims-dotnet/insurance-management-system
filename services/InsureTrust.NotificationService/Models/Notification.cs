using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsureTrust.NotificationService.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ColorCode { get; set; } = "Blue";  // Green, Yellow, Red, Blue

    public bool IsRead { get; set; } = false;

    [MaxLength(50)]
    public string RelatedFeature { get; set; } = string.Empty;  // Policy, Claim, Support

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}

