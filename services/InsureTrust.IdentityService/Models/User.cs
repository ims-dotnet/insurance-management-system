using System.ComponentModel.DataAnnotations;

namespace InsureTrust.IdentityService.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string UserNumber { get; set; } = string.Empty;  // USR1001

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    [Phone]
    public string PhoneNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$")]
    public string PanCard { get; set; } = string.Empty;

    [MaxLength(500)]
    public string KycDocumentPath { get; set; } = string.Empty;

    [MaxLength(20)]
    public string KycStatus { get; set; } = "Pending";  // Pending, Verified

    [MaxLength(20)]
    public string Role { get; set; } = "Customer";  // Customer, Admin

    [Precision(18, 2)]
    public decimal Balance { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

