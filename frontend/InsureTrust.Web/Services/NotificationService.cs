using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InsureTrust.Web.Services;

public record NotificationDto(int Id, string Title, string Message, string ColorCode, bool IsRead, DateTime CreatedAt);
public record SendNotificationDto(int UserId, string Title, string Message, string ColorCode, string Feature);
public record UnreadCountResponseDto(int Count);

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>?> GetMyNotificationsAsync(string token);
    Task<int> GetUnreadCountAsync(string token);
    Task<bool> MarkReadAsync(int id, string token);
    Task<bool> MarkAllReadAsync(string token);
    Task<bool> SendNotificationAsync(SendNotificationDto request);
}

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;

    public NotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private void AddAuthorizationHeader(HttpRequestMessage request, string token)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IEnumerable<NotificationDto>?> GetMyNotificationsAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/notifications");
            AddAuthorizationHeader(request, token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<NotificationDto>>();
            }
        }
        catch { }

        return null;
    }

    public async Task<int> GetUnreadCountAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/notifications/unread-count");
            AddAuthorizationHeader(request, token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UnreadCountResponseDto>();
                return result?.Count ?? 0;
            }
        }
        catch { }

        return 0;
    }

    public async Task<bool> MarkReadAsync(int id, string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/notifications/mark-read/{id}");
            AddAuthorizationHeader(request, token);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch { }

        return false;
    }

    public async Task<bool> MarkAllReadAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/notifications/mark-all-read");
            AddAuthorizationHeader(request, token);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch { }

        return false;
    }

    public async Task<bool> SendNotificationAsync(SendNotificationDto requestDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/notifications/send", requestDto);
            return response.IsSuccessStatusCode;
        }
        catch { }

        return false;
    }
}
