using InsureTrust.ProductService.DTOs;
using InsureTrust.ProductService.Repository;

namespace InsureTrust.ProductService.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _repo;
        private readonly HttpClient _httpClient;
        private readonly string _notificationUrl;

        public PolicyService(IPolicyRepository repo, HttpClient httpClient, IConfiguration configuration)
        {
            _repo = repo;
            _httpClient = httpClient;
            _notificationUrl = configuration["ServiceUrls:NotificationUrl"] ?? "https://localhost:7016/api/notifications/send";
        }

        private async Task SendNotification(int userId, string title, string message, string color, string feature)
        {
            try
            {
                var payload = new
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    ColorCode = color,
                    Feature = feature
                };
                await _httpClient.PostAsJsonAsync(_notificationUrl, payload);
            }
            catch { /* Log failure but don't break flow */ }
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

            var result = await _repo.PurchaseAsync(dto, userId);
            
            if (result != null && result.Id > 0)
            {
                await SendNotification(userId, "Policy Purchased", 
                    $"Your request for {result.PolicyTypeName} is pending approval.", 
                    "GoldenRod", "Policy");
            }

            return result;
        }

        public async Task<PolicyDto> ApprovePolicyAsync(int policyId, ApprovePolicyDto dto, int adminId)
        {
            if (policyId <= 0 || dto == null)
                return new PolicyDto();

            dto.Action = dto.Action?.Trim() ?? "Reject";

            var result = await _repo.ApprovePolicyAsync(policyId, dto, adminId);
            
            if (result != null)
            {
                string status = result.Status ?? "Processed";
                string color = status.ToLower() == "active" ? "Green" : "Red";
                string message = status.ToLower() == "active" 
                    ? $"Your policy {result.PolicyNumber} has been approved." 
                    : $"Your policy request {result.PolicyNumber} was rejected. Reason: {dto.AdminRemarks}";

                await SendNotification(result.UserId, $"Policy {status}", message, color, "Policy");
                
                // Also notify admin of their own action as requested
                await SendNotification(adminId, "Action Completed", $"You have {status.ToLower()} policy {result.PolicyNumber}.", "Blue", "AdminAction");
            }

            return result ?? new PolicyDto();
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