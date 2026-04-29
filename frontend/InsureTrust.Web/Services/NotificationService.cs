using System.Net.Http.Headers;
using InsureTrust.Web.Models;
using System.Net.Http.Json;

namespace InsureTrust.Web.Services;

public record NotificationDto(int Id, string Title, string Message, string ColorCode, bool IsRead, DateTime CreatedAt);
public record SendNotificationDto(int UserId, string Title, string Message, string ColorCode, string Feature);
public record UnreadCountResponseDto(int Count);

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>?> GetMyNotificationsAsync();
    Task<int> GetUnreadCountAsync();
    Task<bool> MarkReadAsync(int id);
    Task<bool> MarkAllReadAsync();
    Task<bool> SendNotificationAsync(SendNotificationDto request);
}

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NotificationService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AddAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["authToken"];
        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IEnumerable<NotificationDto>?> GetMyNotificationsAsync()
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("/api/notifications");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<NotificationDto>>();
            }
        }
        catch { }

        return null;
    }

    public async Task<int> GetUnreadCountAsync()
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("/api/notifications/unread-count");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UnreadCountResponseDto>();
                return result?.Count ?? 0;
            }
        }
        catch { }

        return 0;
    }

    public async Task<bool> MarkReadAsync(int id)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsync($"/api/notifications/mark-read/{id}", null);
            return response.IsSuccessStatusCode;
        }
        catch { }

        return false;
    }

    public async Task<bool> MarkAllReadAsync()
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsync("/api/notifications/mark-all-read", null);
            return response.IsSuccessStatusCode;
        }
        catch { }

        return false;
    }

    public async Task<bool> SendNotificationAsync(SendNotificationDto requestDto)
    {
        try
        {
            // Internal send endpoint is [AllowAnonymous]
            var response = await _httpClient.PostAsJsonAsync("/api/notifications/send", requestDto);
            return response.IsSuccessStatusCode;
        }
        catch { }

        return false;
    }
}
