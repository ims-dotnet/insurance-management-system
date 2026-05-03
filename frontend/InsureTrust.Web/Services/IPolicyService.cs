using InsureTrust.Web.Models;
﻿using InsureTrust.Web.Models;

namespace InsureTrust.Web.Services
{
    public interface IPolicyService
    {
        Task<IEnumerable<PolicyTypeDto>> GetAllPolicyTypeAsync();

        Task<PolicyTypeDto?> GetPolicyTypeByIdAsync(int id);

        Task<PolicyDto?> PurchaseAsync(CreatePolicyDto dto);

        Task<IEnumerable<PolicyDto>> GetAllPolicy();

        Task<IEnumerable<PolicyDto>> GetAllPolicybyid();
        public Task<IEnumerable<PolicyDto>> GetAllPending();
        public Task<bool> EditPolicy(CreatePolicyDto dto, int policyId);

        public Task<bool> CreateAsync(CreatePolicyTypeDto dto);
        Task<bool> Delete(int id);

        Task<bool> UpdatePolicyTypeAsync(int id, PolicyTypeDto dto);
        public Task<bool> RenewPolicyAsync(int policyid,int userid);
    }
}