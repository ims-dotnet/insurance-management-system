using InsureTrust.ClaimService.Data;
using InsureTrust.ClaimService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.ClaimService.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimDbContext _context;

        public ClaimRepository(ClaimDbContext context)
        {
            _context = context;
        }

        public async Task<Claim> AddAsync(Claim claim)
        {
            await _context.Claims.AddAsync(claim);
            return claim;
        }

        public async Task<IEnumerable<Claim>> GetByUserIdAsync(int userId)
        {
            return await _context.Claims
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _context.Claims
                .OrderByDescending(c => c.SubmittedAt)
                .ToListAsync();
        }

        public async Task<Claim?> GetByIdAsync(int id)
        {
            return await _context.Claims.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetLastClaimNumberAsync()
        {
            var lastClaim = await _context.Claims
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();
            return lastClaim?.ClaimNumber;
        }
    }
}