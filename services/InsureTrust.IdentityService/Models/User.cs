namespace InsureTrust.IdentityService.Models;

public class User
{
    public int Id { get; set; }
    public string UserNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public string PanCard { get; set; } = string.Empty;
    public string KycDocumentPath { get; set; } = string.Empty;
    public string KycStatus { get; set; } = "Pending";
    public string Role { get; set; } = "Customer";
    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
