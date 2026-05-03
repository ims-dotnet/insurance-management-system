using InsureTrust.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Json;

namespace InsureTrust.Web.Services
{
    public class ClaimService : IClaimService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddToken()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["authToken"];
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IEnumerable<ClaimViewModel>> GetMyClaimsAsync()
        {
            AddToken();
            var response = await _httpClient.GetAsync("api/claim/my-claims");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                JsonElement dataProperty = default;
                bool hasData = apiResponse.ValueKind == JsonValueKind.Object && apiResponse.TryGetProperty("data", out dataProperty);
                var rawJson = hasData ? dataProperty.GetRawText() : apiResponse.GetRawText();
                
                return JsonSerializer.Deserialize<List<ClaimViewModel>>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ClaimViewModel>();
            }
            return new List<ClaimViewModel>();
        }

        public async Task<bool> SubmitClaimAsync(int policyId, SubmitClaimViewModel model)
        {
            AddToken();
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.MaturityAmount.ToString()), "MaturityAmount");

            var response = await _httpClient.PostAsync($"api/claim/submit/{policyId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<AdminClaimViewModel>> GetAllClaimsAsync()
        {
            AddToken();
            var response = await _httpClient.GetAsync("api/claim/all");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                JsonElement dataProperty = default;
                bool hasData = apiResponse.ValueKind == JsonValueKind.Object && apiResponse.TryGetProperty("data", out dataProperty);
                var rawJson = hasData ? dataProperty.GetRawText() : apiResponse.GetRawText();
                
                return JsonSerializer.Deserialize<List<AdminClaimViewModel>>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminClaimViewModel>();
            }
            return new List<AdminClaimViewModel>();
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, string action, string remarks)
        {
            AddToken();
            var payload = new { Action = action, AdminRemarks = remarks ?? string.Empty };
            var response = await _httpClient.PutAsJsonAsync($"api/claim/{claimId}", payload);
            return response.IsSuccessStatusCode;
        }
    }
}
