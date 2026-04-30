using AutoMapper;
using InsureTrust.ProductService.Data;
using InsureTrust.ProductService.DTOs;
using InsureTrust.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.ProductService.Repository
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly InsureTrustProductServiceContext _context;
        private readonly IMapper _mapper;

        public PolicyRepository(InsureTrustProductServiceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

      

        public async Task<IEnumerable<PolicyTypeDto>> GetPolicyTypesAsync()
        {
            var data = await _context.PolicyTypes
                .Include(x => x.Terms)
                .Include(x => x.RequiredFields)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PolicyTypeDto>>(data);
        }

        public async Task<PolicyTypeDto?> GetPolicyTypeByIdAsync(int id)
        {
            var entity = await _context.PolicyTypes
                .Include(x => x.Terms)
                .Include(x => x.RequiredFields)
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? null : _mapper.Map<PolicyTypeDto>(entity);
        }

        public async Task<PolicyTypeDto> CreatePolicyTypeAsync(CreatePolicyTypeDto dto)
        {
            var entity = _mapper.Map<PolicyType>(dto);

            _context.PolicyTypes.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<PolicyTypeDto>(entity);
        }

        public async Task<PolicyTypeDto?> UpdatePolicyTypeAsync(int id, CreatePolicyTypeDto dto)
        {
            var entity = await _context.PolicyTypes.FindAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);

            await _context.SaveChangesAsync();

            return _mapper.Map<PolicyTypeDto>(entity);
        }

        public async Task DeletePolicyTypeAsync(int id)
        {
            var entity = await _context.PolicyTypes.FindAsync(id);

            if (entity == null)
                throw new Exception("Policy type not found");

            var isUsed = await _context.UserPolicies
                .AnyAsync(x => x.PolicyTypeId == id);

            if (isUsed)
                throw new Exception("Cannot delete. Policy is already in use.");

            _context.PolicyTypes.Remove(entity);
            await _context.SaveChangesAsync();
        }

      

        public async Task<IEnumerable<PolicyDto>> GetMyPoliciesAsync(int userId)
        {
            var data = await _context.UserPolicies
                .Where(x => x.UserId == userId)
                .Include(x => x.PolicyType)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PolicyDto>>(data);
        }

        public async Task<IEnumerable<PolicyDto>> GetAllPoliciesAsync()
        {
            var data = await _context.UserPolicies
                .Include(x => x.PolicyType)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PolicyDto>>(data);
        }

        public async Task<IEnumerable<PolicyDto>> GetPendingPoliciesAsync()
        {
            var data = await _context.UserPolicies
                .Where(x => x.Status == "Pending")
                .ToListAsync();

            return _mapper.Map<IEnumerable<PolicyDto>>(data);
        }

        public async Task<PolicyDto> PurchaseAsync(CreatePolicyDto dto, int userId)
        {
            var entity = _mapper.Map<UserPolicy>(dto);

            entity.UserId = userId;
            entity.PolicyNumber = Guid.NewGuid().ToString("N").Substring(0, 8);
            entity.PurchaseDate = DateTime.UtcNow;
            entity.ExpiryDate = DateTime.UtcNow.AddMonths(dto.Tenure);

            _context.UserPolicies.Add(entity);
            await _context.SaveChangesAsync();

            await _context.Entry(entity).Reference(x => x.PolicyType).LoadAsync();

            return _mapper.Map<PolicyDto>(entity);
        }

        public async Task<PolicyDto?> ApprovePolicyAsync(int policyId, ApprovePolicyDto dto, int adminId)
        {
            var entity = await _context.UserPolicies.FindAsync(policyId);
            if (entity == null) return null;

            entity.Status = (dto.Action == "Grant" || dto.Action == "Active") ? "Active" : "Rejected";
            entity.AdminRemarks = dto.AdminRemarks;

            await _context.SaveChangesAsync();

            return _mapper.Map<PolicyDto>(entity);
        }

        public async Task<PolicyDto?> EditPolicyAsync(int policyId, EditPolicyDto dto, int userId)
        {
            var entity = await _context.UserPolicies.FindAsync(policyId);
            if (entity == null) return null;

            _mapper.Map(dto, entity);

            if (dto.Tenure.HasValue)
            {
                entity.ExpiryDate = entity.PurchaseDate.AddMonths(dto.Tenure.Value);
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<PolicyDto>(entity);
        }

        public async Task DeletePolicyAsync(int policyId, int userId, string role)
        {
            var entity = await _context.UserPolicies.FindAsync(policyId);
            if (entity != null)
            {
                _context.UserPolicies.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PolicyDto?> RenewPolicyAsync(int policyId, int userId)
        {
            var entity = await _context.UserPolicies.FindAsync(policyId);
            if (entity == null) return null;

            entity.ExpiryDate = entity.ExpiryDate.AddMonths(entity.Tenure);

            await _context.SaveChangesAsync();

            return _mapper.Map<PolicyDto>(entity);
        }
    }
}