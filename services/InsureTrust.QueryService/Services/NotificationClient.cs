using InsureTrust.QueryService.DTOs;
using System.Text;
using System.Text.Json;

namespace InsureTrust.QueryService.Services
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
            // Prefer GatewayUrl for service-to-service communication (Audit S9)
            var gatewayUrl = _configuration["Services:GatewayUrl"];
            var baseUrl = !string.IsNullOrWhiteSpace(gatewayUrl) ? gatewayUrl : _configuration["Services:IdentityServiceBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogWarning("Notification target URL (Gateway or IdentityService) is not configured.");
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
                // Both internal IdentityService and Gateway match /api/notifications/send
                var response = await _httpClient.PostAsync($"{baseUrl.TrimEnd('/')}/api/notifications/send", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send notification for support ticket {TicketNumber}", ticketNumber);
            }
        }
    }
}