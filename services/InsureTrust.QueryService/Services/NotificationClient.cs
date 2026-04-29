
using InsureTrust.SupportService.DTOs;
using InsureTrust.SupportService.Services;
using System.Text;
using System.Text.Json;

namespace InsureTrust.SupportService.Services
{
    public class NotificationClient : INotificationClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationClient> _logger;

        public NotificationClient(HttpClient httpClient, IConfiguration configuration, ILogger<NotificationClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendSupportStatusChangedAsync(int userId, string ticketNumber, string status)
        {
            var baseUrl = _configuration["Services:IdentityServiceBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogWarning("Identity service base URL is not configured.");
                return;
            }

            var dto = new SendNotificationDto
            {
                UserId = userId,
                Title = "Support Ticket Update",
                Message = $"Your support ticket #{ticketNumber} is now {status}",
                ColorCode = "Blue",
                Feature = "Support"
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{baseUrl}/api/notifications/send", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send notification for support ticket {TicketNumber}", ticketNumber);
            }
        }
    }
}