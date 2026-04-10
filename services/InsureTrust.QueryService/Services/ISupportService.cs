using InsureTrust.ClaimService.DTOs;

namespace InsureTrust.QueryService.Services;


public interface ISupportService
{
    Task<IEnumerable<SupportQueryDto>> GetMyQueriesAsync(int userId);
    Task<IEnumerable<SupportQueryDto>> GetAllQueriesAsync();
    Task<SupportQueryDto> SubmitQueryAsync(CreateSupportQueryDto dto, int userId, string uploadPath);
    Task<SupportQueryDto> UpdateStatusAsync(int ticketId, UpdateSupportStatusDto dto);
}

