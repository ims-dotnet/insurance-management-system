using InsureTrust.QueryService.Models;

namespace InsureTrust.QueryService.Repositories
{
    public interface ISupportRepository
    {
        Task<SupportQuery?> GetByIdAsync(int id);
        Task<SupportQuery?> GetLatestAsync();
        Task<List<SupportQuery>> GetByUserIdAsync(int userId);
        Task<List<SupportQuery>> GetAllAsync();
        Task AddAsync(SupportQuery query);
        Task UpdateAsync(SupportQuery query);
        Task SaveChangesAsync();
    }
}