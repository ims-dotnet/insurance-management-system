using InsureTrust.NotificationService.DTOs;

namespace InsureTrust.NotificationService.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkReadAsync(int notifId, int userId);
    Task MarkAllReadAsync(int userId);
    Task SendAsync(int userId, string title, string message, string color, string feature);
}

