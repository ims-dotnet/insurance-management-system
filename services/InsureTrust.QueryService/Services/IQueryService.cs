using InsureTrust.QueryService.DTOs;

namespace InsureTrust.QueryService.Services
{
    public interface IQueryService
    {
        Task<IEnumerable<SupportQueryDto>> GetMyQueriesAsync(int userId);
        Task<IEnumerable<SupportQueryDto>> GetAllQueriesAsync();
        Task<SupportQueryDto> SubmitQueryAsync(CreateSupportQueryDto dto, int userId, string webRootPath);
        Task<SupportQueryDto> UpdateStatusAsync(int ticketId, UpdateSupportStatusDto dto);
    }
}