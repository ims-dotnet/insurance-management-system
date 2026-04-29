using InsureTrust.ProductService.DTOs;
using InsureTrust.ProductService.Repository;

namespace InsureTrust.ProductService.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _repo;

        public PolicyService(IPolicyRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<PolicyTypeDto>> GetPolicyTypesAsync()
        {
            var data = await _repo.GetPolicyTypesAsync();
            return data?.Where(x => x.Id > 0).ToList() ?? new List<PolicyTypeDto>();
        }

        public async Task<PolicyTypeDto> GetPolicyTypeByIdAsync(int id)
        {
            if (id <= 0) return new PolicyTypeDto();
            return await _repo.GetPolicyTypeByIdAsync(id) ?? new PolicyTypeDto();
        }

        public async Task<PolicyTypeDto> CreatePolicyTypeAsync(CreatePolicyTypeDto dto)
        {
            if (dto == null) return new PolicyTypeDto();

            dto.Name = dto.Name?.Trim() ?? "Default Policy";
            dto.Category = dto.Category?.Trim() ?? "General";

            return await _repo.CreatePolicyTypeAsync(dto);
        }

        public async Task<PolicyTypeDto> UpdatePolicyTypeAsync(int id, CreatePolicyTypeDto dto)
        {
            if (id <= 0) return new PolicyTypeDto();
            return await _repo.UpdatePolicyTypeAsync(id, dto) ?? new PolicyTypeDto();
        }

        public async Task DeletePolicyTypeAsync(int id)
        {
            if (id <= 0) return;
            await _repo.DeletePolicyTypeAsync(id);
        }

        public async Task<IEnumerable<PolicyDto>> GetMyPoliciesAsync(int userId)
        {
            if (userId <= 0) return new List<PolicyDto>();
            var data = await _repo.GetMyPoliciesAsync(userId);
            return data?.OrderByDescending(x => x.Id).ToList() ?? new List<PolicyDto>();
        }

        public async Task<IEnumerable<PolicyDto>> GetAllPoliciesAsync()
        {
            var data = await _repo.GetAllPoliciesAsync();
            return data?.OrderByDescending(x => x.Id).ToList() ?? new List<PolicyDto>();
        }

        public async Task<IEnumerable<PolicyDto>> GetPendingPoliciesAsync()
        {
            var data = await _repo.GetPendingPoliciesAsync();
            return data?.ToList() ?? new List<PolicyDto>();
        }

        public async Task<PolicyDto> PurchaseAsync(CreatePolicyDto dto, int userId)
        {
            if (dto == null || userId <= 0)
                return new PolicyDto();

            if (dto.Tenure < 0) dto.Tenure = 0;

            return await _repo.PurchaseAsync(dto, userId);
        }

        public async Task<PolicyDto> ApprovePolicyAsync(int policyId, ApprovePolicyDto dto, int adminId)
        {
            if (policyId <= 0 || dto == null)
                return new PolicyDto();

            dto.Action = dto.Action?.Trim() ?? "Reject";

            return await _repo.ApprovePolicyAsync(policyId, dto, adminId) ?? new PolicyDto();
        }

        public async Task<PolicyDto> EditPolicyAsync(int policyId, EditPolicyDto dto, int userId)
        {
            if (policyId <= 0 || dto == null)
                return new PolicyDto();

            return await _repo.EditPolicyAsync(policyId, dto, userId) ?? new PolicyDto();
        }

        public async Task DeletePolicyAsync(int policyId, int userId, string role)
        {
            if (role?.ToLower() != "admin") return;

            await _repo.DeletePolicyAsync(policyId, userId, role);
        }

        public async Task<PolicyDto> RenewPolicyAsync(int policyId, int userId)
        {
            if (policyId <= 0) return new PolicyDto();

            return await _repo.RenewPolicyAsync(policyId, userId) ?? new PolicyDto();
        }
    }
}