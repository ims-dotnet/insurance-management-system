using InsureTrust.Web.Models;

namespace InsureTrust.Web.Services
{
    public interface IClaimService
    {
        Task<IEnumerable<ClaimViewModel>> GetMyClaimsAsync();
        Task<bool> SubmitClaimAsync(int policyId, SubmitClaimViewModel model);
    }
}
