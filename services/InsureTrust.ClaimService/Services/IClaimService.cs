using InsureTrust.ClaimService.DTOs;

namespace InsureTrust.ClaimService.Services;

public interface IClaimService
{
    Task<IEnumerable<ClaimDto>> GetMyClaimsAsync(int userId);
    Task<IEnumerable<ClaimDto>> GetAllClaimsAsync();
    Task<ClaimDto> SubmitClaimAsync(int policyId, SubmitClaimDto dto, int userId, string uploadPath);
    Task<ClaimDto> UpdateClaimAsync(int claimId, UpdateClaimDto dto);
}

