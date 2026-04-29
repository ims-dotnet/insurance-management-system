using InsureTrust.Web.Models;

namespace InsureTrust.Web.Services
{
    public interface ISupportService
    {
        Task<ApiResponse<SupportQueryViewModel>?> SubmitSupportQueryAsync(MultipartFormDataContent content);
        Task<ApiResponse<List<SupportQueryViewModel>>?> GetMyQueriesAsync();
        Task<ApiResponse<List<SupportQueryViewModel>>?> GetAllQueriesAsync();
        Task<ApiResponse<object>?> UpdateQueryStatusAsync(int id, UpdateSupportStatusViewModel model);
    }
}
