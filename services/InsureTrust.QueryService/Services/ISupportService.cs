using InsureTrust.SupportService.DTOs;

namespace InsureTrust.SupportService.Services
{
    public interface ISupportService
    {
        Task<IEnumerable<SupportQueryDto>> GetMyQueriesAsync(int userId);
        Task<IEnumerable<SupportQueryDto>> GetAllQueriesAsync();
        Task<SupportQueryDto> SubmitQueryAsync(CreateSupportQueryDto dto, int userId, string webRootPath);
        Task<SupportQueryDto> UpdateStatusAsync(int ticketId, UpdateSupportStatusDto dto);
    }
}