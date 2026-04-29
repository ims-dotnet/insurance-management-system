using InsureTrust.AdminService.DTOs;
using System.Net.Http.Json;

namespace InsureTrust.AdminService.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;

        public AdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 🔹 Dashboard
        public async Task<DashboardStatsDto> GetDashboardAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<List<AdminUserDto>>("api/auth/users");

            var policies = await _httpClient.GetFromJsonAsync<List<dynamic>>("api/policy/all");

            var claims = await _httpClient.GetFromJsonAsync<List<dynamic>>("api/claim/all");

            var payments = await _httpClient.GetFromJsonAsync<List<AdminTransactionDto>>("api/payment/history");

            return new DashboardStatsDto
            {
                TotalUsers = users?.Count ?? 0,
                TotalActivePolicies = policies?.Count(p => p.status == "Active") ?? 0,
                TotalPendingPolicies = policies?.Count(p => p.status == "Pending") ?? 0,
                TotalPendingClaims = claims?.Count(c => c.claimStatus == "Pending") ?? 0,
                TotalOpenSupportTickets = 0,
                TotalUnreadNotifications = 0,
                TotalRevenue = payments?.Sum(p => p.Amount) ?? 0
            };
        }

        // 🔹 Users
        public async Task<IEnumerable<AdminUserDto>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AdminUserDto>>("api/auth/users")
                   ?? new List<AdminUserDto>();
        }

        // 🔹 Transactions
        public async Task<IEnumerable<AdminTransactionDto>> GetTransactionsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AdminTransactionDto>>("api/payment/history")
                   ?? new List<AdminTransactionDto>();
        }
    }
}