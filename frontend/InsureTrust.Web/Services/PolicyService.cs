using InsureTrust.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InsureTrust.Web.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _context;

        public PolicyService(HttpClient http, IHttpContextAccessor context)
        {
            _http = http;
            _context = context;
        }

        private void AddToken()
        {
            var token = _context.HttpContext?.Request.Cookies["authToken"];
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim().Replace("\r", "").Replace("\n", "");
                if (token.StartsWith("Bearer ")) token = token.Substring(7);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<T?> GetFromApiAsync<T>(string url, bool needsAuth = false)
        {
            if (needsAuth) AddToken();
            try
            {
                var response = await _http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                    return apiResponse != null ? apiResponse.Data : default;
                }
            }
            catch { }
            return default;
        }

        public async Task<IEnumerable<PolicyTypeDto>> GetAllPolicyTypeAsync()
        {
            var data = await GetFromApiAsync<IEnumerable<PolicyTypeDto>>("api/policy/types");
            return data ?? new List<PolicyTypeDto>();
        }

        public async Task<PolicyTypeDto?> GetPolicyTypeByIdAsync(int id)
        {
            return await GetFromApiAsync<PolicyTypeDto>($"api/policy/types/{id}");
        }

        public async Task<PolicyDto?> PurchaseAsync(CreatePolicyDto dto)
        {
            AddToken();
            var response = await _http.PostAsJsonAsync("api/policy/purchase", dto);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PolicyDto>>();
                return apiResponse?.Data;
            }
            return null;
        }

        public async Task<bool> EditPolicy(CreatePolicyDto dto, int policyId)
        {
            AddToken();
            var editDto = new { Tenure = dto.Tenure, PackageAmount = dto.PackageAmount };
            var response = await _http.PutAsJsonAsync($"api/policy/edit/{policyId}", editDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateAsync(CreatePolicyTypeDto dto)
        {
            AddToken();
            var response = await _http.PostAsJsonAsync("api/policy/types", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<PolicyDto>> GetAllPolicybyid()
        {
            var data = await GetFromApiAsync<IEnumerable<PolicyDto>>("api/policy/my", true);
            return data ?? new List<PolicyDto>();
        }

        public async Task<IEnumerable<PolicyDto>> GetAllPolicy()
        {
            var data = await GetFromApiAsync<IEnumerable<PolicyDto>>("api/policy/all", true);
            return data ?? new List<PolicyDto>();
        }

        public async Task<IEnumerable<PolicyDto>> GetAllPending()
        {
            var data = await GetFromApiAsync<IEnumerable<PolicyDto>>("api/policy/pending", true);
            return data ?? new List<PolicyDto>();
        }

        public async Task<bool> Delete(int id)
        {
            AddToken();
            var response = await _http.DeleteAsync($"api/policy/types/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePolicyTypeAsync(int id, PolicyTypeDto dto)
        {
            AddToken();
            var updateDto = new CreatePolicyTypeDto
            {
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                Icon = dto.Icon
            };
            var response = await _http.PutAsJsonAsync($"api/policy/types/{id}", updateDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RenewPolicyAsync(int policyId, int userId)
        {
            AddToken();
            var response = await _http.PostAsync($"api/policy/renew/{policyId}", null);
            return response.IsSuccessStatusCode;
        }
    }
}