using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Services
{
    public interface IProductServiceClient
    {
        Task<ProductPolicyDto?> GetPolicyByPolicyIdAsync(int policyId);
        Task<ProductPolicyDto?> GetPolicyByNumberAsync(string policyNumber);
        Task<bool> RenewPolicyByNumberAsync(string policyNumber);
        Task<ProductPolicyDto?> RegisterNewPolicyAsync(int userId, int policyId, decimal amount);
    }
}