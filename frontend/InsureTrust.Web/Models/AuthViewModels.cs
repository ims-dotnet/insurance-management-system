using System.ComponentModel.DataAnnotations;

namespace InsureTrust.Web.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string LoginMode { get; set; } = "user";

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required]
        public string PhoneNo { get; set; } = string.Empty;
        
        [Required]
        public string PanCard { get; set; } = string.Empty;
        
        public IFormFile? KycDocument { get; set; }
    }

    public class UpdateProfileViewModel
    {
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public IFormFile? KycDocument { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto? User { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string UserNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
