using AutoMapper;
using InsureTrust.SupportService.DTOs;
using InsureTrust.SupportService.Exceptions;
using InsureTrust.SupportService.Helpers;
using InsureTrust.SupportService.Models;
using InsureTrust.SupportService.Repositories;

namespace InsureTrust.SupportService.Services
{
    public class SupportService : ISupportService
    {

        private readonly ISupportRepository _supportRepository;
        private readonly INotificationClient _notificationClient;
        private readonly IMapper _mapper;
        private readonly ILogger<SupportService> _logger;

        public SupportService(
            ISupportRepository supportRepository,
            INotificationClient notificationClient,
            IMapper mapper,
            ILogger<SupportService> logger)
        {
            _supportRepository = supportRepository;
            _notificationClient = notificationClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SupportQueryDto>> GetMyQueriesAsync(int userId)
        {
            var queries = await _supportRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<SupportQueryDto>>(queries);
        }

        public async Task<IEnumerable<SupportQueryDto>> GetAllQueriesAsync()
        {
            var queries = await _supportRepository.GetAllAsync();
            return _mapper.Map<List<SupportQueryDto>>(queries);
        }

        public async Task<SupportQueryDto> SubmitQueryAsync(CreateSupportQueryDto dto, int userId, string webRootPath)
        {
            FileValidationHelper.Validate(dto.Attachment, nameof(dto.Attachment));

            var latest = await _supportRepository.GetLatestAsync();
            var ticketNumber = TicketNumberGenerator.Generate(latest?.TicketNumber);
            var attachmentPath = await FileValidationHelper.SaveFileAsync(dto.Attachment, webRootPath, "support-attachments");

            var supportQuery = _mapper.Map<SupportQuery>(dto);
            supportQuery.TicketNumber = ticketNumber;
            supportQuery.UserId = userId;
            supportQuery.Status = "Pending";
            supportQuery.AttachmentPath = attachmentPath;
            supportQuery.CreatedAt = DateTime.UtcNow;

            await _supportRepository.AddAsync(supportQuery);
            await _supportRepository.SaveChangesAsync();

            return _mapper.Map<SupportQueryDto>(supportQuery);
        }

        public async Task<SupportQueryDto> UpdateStatusAsync(int ticketId, UpdateSupportStatusDto dto)
        {
            var query = await _supportRepository.GetByIdAsync(ticketId);
            if (query == null)
                throw new NotFoundException("Support ticket not found.");

            query.Status = dto.Status;
            query.AdminResponse = dto.AdminResponse;

            if (dto.Status == "Resolved")
                query.ResolvedAt = DateTime.UtcNow;
            else
                query.ResolvedAt = null;

            await _supportRepository.UpdateAsync(query);
            await _supportRepository.SaveChangesAsync();

            try
            {
                await _notificationClient.SendSupportStatusChangedAsync(query.UserId, query.TicketNumber, query.Status);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send support status update notification for Ticket: {TicketNumber}", query.TicketNumber);
            }

            return _mapper.Map<SupportQueryDto>(query);
        }
    }
}