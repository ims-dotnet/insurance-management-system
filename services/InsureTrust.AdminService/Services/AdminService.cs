using InsureTrust.AdminService.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace InsureTrust.AdminService.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<DashboardStatsDto> GetDashboardAsync()
        {
            AddAuthorizationHeader();

            var stats = new DashboardStatsDto();

            try
            {
                // Users
                var usersResp = await _httpClient.GetAsync("api/auth/users");
                if (usersResp.IsSuccessStatusCode)
                {
                    var content = await usersResp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                        stats.TotalUsers = data.GetArrayLength();
                }

                // Policies
                var polResp = await _httpClient.GetAsync("api/policy/all");
                if (polResp.IsSuccessStatusCode)
                {
                    var content = await polResp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        var policies = data.Deserialize<List<AdminPolicyDto>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        stats.TotalActivePolicies = policies?.Count(p => p.Status == "Active") ?? 0;
                        stats.TotalPendingPolicies = policies?.Count(p => p.Status == "Pending") ?? 0;
                    }
                }

                // Claims
                var claimResp = await _httpClient.GetAsync("api/claim/all");
                if (claimResp.IsSuccessStatusCode)
                {
                    var content = await claimResp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        var claims = data.Deserialize<List<AdminClaimDto>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        stats.TotalPendingClaims = claims?.Count(c => c.ClaimStatus == "Pending") ?? 0;
                    }
                }

                // Payments
                var payResp = await _httpClient.GetAsync("api/payment/history");
                if (payResp.IsSuccessStatusCode)
                {
                    var content = await payResp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        var payments = data.Deserialize<List<AdminTransactionDto>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        stats.TotalRevenue = payments?.Sum(p => p.Amount) ?? 0;
                    }
                }

                // Support Tickets
                var supportResp = await _httpClient.GetAsync("api/queries/all");
                if (supportResp.IsSuccessStatusCode)
                {
                    var content = await supportResp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        var tickets = data.EnumerateArray();
                        stats.TotalOpenSupportTickets = tickets.Count(t => t.GetProperty("status").GetString() != "Resolved");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }

            return stats;
        }

        // 🔹 Users
        public async Task<IEnumerable<AdminUserDto>> GetUsersAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/auth/users");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("data", out var data))
                {
                    return data.Deserialize<List<AdminUserDto>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminUserDto>();
                }
            }
            return new List<AdminUserDto>();
        }

        // 🔹 Transactions
        public async Task<IEnumerable<AdminTransactionDto>> GetTransactionsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/payment/history");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("data", out var data))
                {
                    return data.Deserialize<List<AdminTransactionDto>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AdminTransactionDto>();
                }
            }
            return new List<AdminTransactionDto>();
        }
    }
}