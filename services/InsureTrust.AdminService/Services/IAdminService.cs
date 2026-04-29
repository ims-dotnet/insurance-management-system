using InsureTrust.AdminService.DTOs;

namespace InsureTrust.AdminService.Services
{
    public interface IAdminService
    {
        Task<DashboardStatsDto> GetDashboardAsync();
        Task<IEnumerable<AdminUserDto>> GetUsersAsync();
        Task<IEnumerable<AdminTransactionDto>> GetTransactionsAsync();
    }
}