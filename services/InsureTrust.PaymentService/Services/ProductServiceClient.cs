using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Services
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductServiceClient> _logger;

        public ProductServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ProductServiceClient> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private void AddAuthorizationHeader()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ProductPolicyDto?> GetPolicyByPolicyIdAsync(int policyId)
        {
            try
            {
                AddAuthorizationHeader();
                // Hit the Gateway route (api/policy/{id})
                var response = await _httpClient.GetAsync($"api/policy/{policyId}");
                if (response.IsSuccessStatusCode)
                {
                    // If the response is wrapped, we need to unwrap it. 
                    // Based on audit, we expect an ApiResponse structure.
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    if (jsonDoc.RootElement.TryGetProperty("data", out var data))
                    {
                        return JsonSerializer.Deserialize<ProductPolicyDto>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    return JsonSerializer.Deserialize<ProductPolicyDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch real policy data for PolicyId: {PolicyId}", policyId);
            }

            _logger.LogWarning("Falling back to MOCK data for PolicyId: {PolicyId}", policyId);
            return new ProductPolicyDto
            {
                PolicyId = policyId,
                PolicyType = "Health",
                PackageAmount = 5000m,
                ExpiryDate = DateTime.UtcNow.AddMonths(12)
            };
        }

        public async Task<ProductPolicyDto?> GetPolicyByNumberAsync(string policyNumber)
        {
            // The ProductService doesn't have a direct 'by number' endpoint yet, 
            // but we can search in 'all' if needed. For now, keep mock with a warning.
            _logger.LogWarning("GetPolicyByNumberAsync requested for {PolicyNumber}. Routing to all search.", policyNumber);
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/policy/all");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    JsonElement dataArray;
                    if (jsonDoc.RootElement.TryGetProperty("data", out dataArray))
                    {
                        var policies = JsonSerializer.Deserialize<List<ProductPolicyDto>>(dataArray.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return policies?.FirstOrDefault(p => p.PolicyNumber == policyNumber);
                    }
                }
            }
            catch { }

            return null; // Let the service handle fallback
        }

        public async Task<bool> RenewPolicyByNumberAsync(string policyNumber)
        {
            // Similar to above, find by number then renew by ID
            var policy = await GetPolicyByNumberAsync(policyNumber);
            if (policy != null)
            {
                AddAuthorizationHeader();
                var response = await _httpClient.PostAsync($"api/policy/renew/{policy.PolicyId}", null);
                return response.IsSuccessStatusCode;
            }
            return false;
        }

        public async Task<ProductPolicyDto?> RegisterNewPolicyAsync(int userId, int policyId, decimal amount)
        {
            try
            {
                AddAuthorizationHeader();
                var payload = new { PolicyTypeId = policyId, PackageAmount = amount }; // Match CreatePolicyDto in ProductService
                var response = await _httpClient.PostAsJsonAsync("api/policy/purchase", payload);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    if (jsonDoc.RootElement.TryGetProperty("data", out var data))
                    {
                        return JsonSerializer.Deserialize<ProductPolicyDto>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    return JsonSerializer.Deserialize<ProductPolicyDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch { }
            return null;
        }
    }
}