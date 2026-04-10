using System.ComponentModel.DataAnnotations;

namespace InsureTrust.IdentityService.DTOs;

public class UpdateProfileDto
{
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Phone]
    [MaxLength(15)]
    public string? PhoneNo { get; set; }

    public IFormFile? KycDocument { get; set; }
}

