using InsureTrust.PaymentService.Models;

namespace InsureTrust.PaymentService.Repositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<List<Payment>> GetByUserIdAsync(int userId);
        Task<List<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
