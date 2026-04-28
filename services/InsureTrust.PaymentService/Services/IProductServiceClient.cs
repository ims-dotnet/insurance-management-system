using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Services
{
    public interface IProductServiceClient
    {
        Task<ProductPolicyDto?> GetPolicyByPolicyIdAsync(int policyId);
        Task<ProductPolicyDto?> GetPolicyAsync(int userPolicyId);
        Task<bool> RenewPolicyAsync(int userPolicyId);
    }
}