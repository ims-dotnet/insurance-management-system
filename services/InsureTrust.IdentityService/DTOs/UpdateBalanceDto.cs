namespace InsureTrust.IdentityService.DTOs;

public class UpdateBalanceDto
{
    public int UserId { get; set; }
    public decimal AmountToDeduct { get; set; }
}
