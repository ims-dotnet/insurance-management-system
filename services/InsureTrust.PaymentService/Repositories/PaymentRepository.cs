using InsureTrust.PaymentService.Data;
using InsureTrust.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.PaymentService.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;
        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }
        public async Task<List<Payment>> GetByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();
        }
        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();
        }
        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments.FindAsync(id);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
