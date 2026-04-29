using InsureTrust.IdentityService.DTOs;
using InsureTrust.IdentityService.Models;

namespace InsureTrust.IdentityService.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPanCardAsync(string panCard);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<IReadOnlyList<User>> GetAllAsync();
        Task SaveChangesAsync();
    }
}