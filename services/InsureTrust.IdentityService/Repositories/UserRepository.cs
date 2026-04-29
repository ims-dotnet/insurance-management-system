using InsureTrust.IdentityService.Data;
using InsureTrust.IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.IdentityService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _dbContext;
        private readonly DbSet<User> _users;

        public UserRepository(IdentityDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            _dbContext = dbContext;
            _users = _dbContext.Set<User>();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return await _users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return await _users.AnyAsync(u => u.Email == normalizedEmail);
        }

        public async Task<bool> ExistsByPanCardAsync(string panCard)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(panCard);
            var normalizedPan = panCard.Trim().ToUpperInvariant();

            return await _users.AnyAsync(u => u.PanCard == normalizedPan);
        }

        public async Task AddAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            await _users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            _users.Update(user);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}