using InsureTrust.ClaimService.Models;

namespace InsureTrust.ClaimService.Repositories
{
    public interface IClaimRepository
    {
        Task<Claim> AddAsync(Claim claim);
        Task<IEnumerable<Claim>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim?> GetByIdAsync(int id);
        Task<string?> GetLastClaimNumberAsync();
        Task SaveChangesAsync();
    }
}