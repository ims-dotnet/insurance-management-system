using InsureTrust.SupportService.Data;
using InsureTrust.SupportService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.SupportService.Repositories
{
    public class SupportRepository : ISupportRepository
    {
        private readonly SupportDbContext _context;

        public SupportRepository(SupportDbContext context)
        {
            _context = context;
        }

        public async Task<SupportQuery?> GetByIdAsync(int id)
        {
            return await _context.SupportQueries.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SupportQuery?> GetLatestAsync()
        {
            return await _context.SupportQueries
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SupportQuery>> GetByUserIdAsync(int userId)
        {
            return await _context.SupportQueries
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<SupportQuery>> GetAllAsync()
        {
            return await _context.SupportQueries
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(SupportQuery query)
        {
            await _context.SupportQueries.AddAsync(query);
        }

        public Task UpdateAsync(SupportQuery query)
        {
            _context.SupportQueries.Update(query);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}