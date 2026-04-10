using InsureTrust.ProductService.DTOs;

namespace InsureTrust.ProductService.Services;

public interface IPolicyService
{
    Task<IEnumerable<PolicyTypeDto>> GetPolicyTypesAsync();
    Task<PolicyTypeDto> GetPolicyTypeByIdAsync(int id);
    Task<PolicyTypeDto> CreatePolicyTypeAsync(CreatePolicyTypeDto dto);
    Task<PolicyTypeDto> UpdatePolicyTypeAsync(int id, CreatePolicyTypeDto dto);
    Task DeletePolicyTypeAsync(int id);
    Task<IEnumerable<PolicyDto>> GetMyPoliciesAsync(int userId);
    Task<IEnumerable<PolicyDto>> GetAllPoliciesAsync();
    Task<IEnumerable<PolicyDto>> GetPendingPoliciesAsync();
    Task<PolicyDto> PurchaseAsync(CreatePolicyDto dto, int userId);
    Task<PolicyDto> ApprovePolicyAsync(int policyId, ApprovePolicyDto dto, int adminId);
    Task<PolicyDto> EditPolicyAsync(int policyId, EditPolicyDto dto, int userId);
    Task DeletePolicyAsync(int policyId, int userId, string role);
    Task<PolicyDto> RenewPolicyAsync(int policyId, int userId);
}

