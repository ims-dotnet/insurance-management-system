using InsureTrust.Productweb.DTOs;
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
            var token = _context.HttpContext?.Session.GetString("JWT");

            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim()
                             .Replace("\r", "")
                             .Replace("\n", "");

                if (token.StartsWith("Bearer "))
                    token = token.Substring(7);

                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IEnumerable<PolicyTypeDto>> GetAllPolicyTypeAsync()
        {
            var data = await _http.GetFromJsonAsync<IEnumerable<PolicyTypeDto>>("api/policy/types");
            return data ?? new List<PolicyTypeDto>();
        }

        public async Task<PolicyTypeDto?> GetPolicyTypeByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<PolicyTypeDto>($"api/policy/types/{id}");
        }

        public async Task<bool> PurchaseAsync(CreatePolicyDto dto)
        {
            AddToken();
            var response = await _http.PostAsJsonAsync("api/policy/purchase", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EditPolicy(CreatePolicyDto dto, int policyId)
        {
            AddToken();

            var editDto = new
            {
                Tenure = dto.Tenure,
                PackageAmount = dto.PackageAmount
            };

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
            AddToken();

            var response = await _http.GetAsync("api/policy/my");

            if (!response.IsSuccessStatusCode)
                return new List<PolicyDto>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<PolicyDto>>() ?? new List<PolicyDto>();
        }

        public async Task<IEnumerable<PolicyDto>> GetAllPolicy()
        {
            AddToken();

            var data = await _http.GetFromJsonAsync<IEnumerable<PolicyDto>>("api/policy/all");
            return data ?? new List<PolicyDto>();
        }

        public async Task<IEnumerable<PolicyDto>> GetAllPending()
        {
            AddToken();

            var data = await _http.GetFromJsonAsync<IEnumerable<PolicyDto>>("api/policy/pending");
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