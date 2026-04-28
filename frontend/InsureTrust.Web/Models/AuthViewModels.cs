using System.ComponentModel.DataAnnotations;

namespace InsureTrust.Web.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string LoginMode { get; set; } = "user";

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string UserNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string KycStatus { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
