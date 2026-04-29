using InsureTrust.AdminService.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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

        // 🔹 Dashboard
        public async Task<DashboardStatsDto> GetDashboardAsync()
        {
            AddAuthorizationHeader();

            var users = await _httpClient.GetFromJsonAsync<List<AdminUserDto>>("api/auth/users");
            var policies = await _httpClient.GetFromJsonAsync<List<AdminPolicyDto>>("api/policy/all");
            var claims = await _httpClient.GetFromJsonAsync<List<AdminClaimDto>>("api/claim/all");
            var payments = await _httpClient.GetFromJsonAsync<List<AdminTransactionDto>>("api/payment/history");

            return new DashboardStatsDto
            {
                TotalUsers = users?.Count ?? 0,
                TotalActivePolicies = policies?.Count(p => p.Status == "Active") ?? 0,
                TotalPendingPolicies = policies?.Count(p => p.Status == "Pending") ?? 0,
                TotalPendingClaims = claims?.Count(c => c.ClaimStatus == "Pending") ?? 0,
                TotalOpenSupportTickets = 0,
                TotalUnreadNotifications = 0,
                TotalRevenue = payments?.Sum(p => p.Amount) ?? 0
            };
        }

        // 🔹 Users
        public async Task<IEnumerable<AdminUserDto>> GetUsersAsync()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<AdminUserDto>>("api/auth/users")
                   ?? new List<AdminUserDto>();
        }

        // 🔹 Transactions
        public async Task<IEnumerable<AdminTransactionDto>> GetTransactionsAsync()
        {
            AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<AdminTransactionDto>>("api/payment/history")
                   ?? new List<AdminTransactionDto>();
        }
    }
}