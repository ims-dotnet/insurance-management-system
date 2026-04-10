using System.ComponentModel.DataAnnotations;

namespace InsureTrust.IdentityService.DTOs;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(15)]
    public string PhoneNo { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$")]
    public string PanCard { get; set; } = string.Empty;

    public IFormFile? KycDocument { get; set; }
}

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
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

